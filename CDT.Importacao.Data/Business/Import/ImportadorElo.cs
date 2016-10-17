using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDT.Importacao.Data.Model;
using System.IO;
using CDT.Importacao.Data.Utils;
using CDT.Importacao.Data.DAL.Classes;
using LAB5;

namespace CDT.Importacao.Data.Business.Import
{
    public class ImportadorElo : IImportador
    {

        InformacaoRegistroDAO infRegistroDAO = new InformacaoRegistroDAO();
        List<InformacaoAux> buffer = new List<InformacaoAux>();
        
        public void Importar(Arquivo arquivo)
        {
            
            StreamReader sr;
            int countLinha = 0;
            List<Registro> registros = arquivo.FK_Layout.Registros.ToList();
            List<Informacao> informacoes;
            if (Directory.Exists(arquivo.FK_Layout.DiretorioArquivo))
            {
                FileInfo fi = new DirectoryInfo(arquivo.FK_Layout.DiretorioArquivo).GetFiles().Where(f=>f.Name.Equals(arquivo.NomeArquivo)).FirstOrDefault();
                informacoes = new List<Informacao>();
                if (fi != null)
                {
                    sr = new StreamReader(fi.OpenRead());
                    string linha = sr.ReadLine();
                    ImportarInformacaoRegisto(registros.Where(r => r.FK_TipoRegistro.NomeTipoRegistro.Equals("Header")).First(), arquivo.IdArquivo, linha, "");
                    linha = "";
                    string copia = ""; 
                    while ((linha = sr.ReadLine()) != null)
                    {
                        
                        countLinha++;
                        if (!TipoTransacaoLinha(linha).Equals("BZ"))
                            ProcessarRegistroDetalhe(registros,arquivo.IdArquivo, ref sr, linha);
                           //ProcessarDetalhes(registros, linha, arquivo.IdArquivo, ref informacoes);
                          // ProcessarDetalhes(registros, linha, arquivo.IdArquivo);

                        copia = linha; 
                    }

                    Finalizar();
                    ImportarInformacaoRegisto(registros.Where(r => r.FK_TipoRegistro.NomeTipoRegistro.Equals("Trailer")).First(), arquivo.IdArquivo, copia,"" );
                    Conciliar(arquivo);
                }
                else
                    throw new IOException("Não existe arquivo com o nome informado");
                
            }
           
        }

        public void Conciliar(Arquivo arquivo)
        {

            List<InformacaoRegistro> info = new InformacaoRegistroDAO().ListarTodos().Where(i => i.IdArquivo == arquivo.IdArquivo).ToList();
            while(info.Count > 0)
            {
                var idTransacoes = from trans in info
                                 group trans by trans.Chave
                                 into transAgrupadas
                                 orderby transAgrupadas.Key
                                 select transAgrupadas.Key;

                foreach(var chave in idTransacoes)
                {
                    var transacoes = info.Where(i => i.Chave.Equals(chave));

                }


                
            }
            //while((info = new InformacaoRegistroDAO().ListarTodos().Where(i=>i.IdArquivo == arquivo.IdArquivo).Take(10000).ToList()) != null)
            //{
            //    foreach(InformacaoRegistro i in info)
            //    {
            //        List<Campo> campos = new CampoDAO().ListarTodos().Where(c => c.IdRegistro == i.IdRegistro).ToList();


            //    }
            //}
        }


        #region Métodos Auxiliares





        private void ImportarInformacaoRegisto(Registro registro, int idArquivo, string linha, string chave)
        {
            new InformacaoRegistroDAO().Salvar(new InformacaoRegistro(registro.IdRegistro, idArquivo ,chave, linha));
        }


        private string ExtrairInformacao(string linha, int ini, int fim)
        {

            return linha.Substring(ini - 1, (fim - ini) + 1);
        }

        private string TipoTransacaoLinha(string linha)
        {
            return ExtrairInformacao(linha, 1, 2);
        }

        private string TipoRegistroTransacao(string linha)
        {
            return ExtrairInformacao(linha, 4, 4);
        }

      

        private void ProcessarRegistroDetalhe(List<Registro> registros,int idArquivo, ref StreamReader reader, string linha)
        {
            string tipoTransacao = TipoTransacaoLinha(linha);

            if (tipoTransacao.Equals(Constantes.TE01))
            {

                TratarRegistroE01(registros, idArquivo, ref reader, linha);
            }
            else
            if  ( tipoTransacao.Equals(Constantes.TE05) || tipoTransacao.Equals(Constantes.TE06) || tipoTransacao.Equals(Constantes.TE15) ||
                             tipoTransacao.Equals(Constantes.TE16) || tipoTransacao.Equals(Constantes.TE25) || tipoTransacao.Equals(Constantes.TE26) || tipoTransacao.Equals(Constantes.TE35) ||
                             tipoTransacao.Equals(Constantes.TE36))
            {

                TratarRegistroE05(registros, idArquivo, ref reader, linha);

            }
            else
                        if (tipoTransacao.Equals(Constantes.TE10))
            {
                TratarRegistroE10(registros, idArquivo, ref reader, linha);
            }
            else
                        if (tipoTransacao.Equals(Constantes.TE20))
            {
                TratarRegistroE20(registros, idArquivo, ref reader, linha);
            }
            else
                        if (tipoTransacao.Equals(Constantes.TE40))
            {

                TratarRegistroE40(registros, idArquivo, ref reader, linha);
            }
           
        }

        public void TratarRegistroE01(List<Registro> registros,int idArquivo, ref StreamReader reader, string linha)
        {
            List<InformacaoRegistro> informacoesRegistro = new List<InformacaoRegistro>();
            string idTransacao = ExtrairInformacao(linha, 27, 49);
         
            informacoesRegistro.Add(new InformacaoRegistro(registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E01_0")).First().IdRegistro, idArquivo, idTransacao, linha));
            for (int i = 1; i < 6; i++)
            {
                linha = reader.ReadLine();
                informacoesRegistro.Add(new InformacaoRegistro(registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E01_" + TipoRegistroTransacao(linha))).First().IdRegistro, idArquivo, idTransacao, linha));
            }

            Persistir(informacoesRegistro);


        }

        public void TratarRegistroE05(List<Registro> registros, int idArquivo, ref StreamReader reader, string linha)
        {
            List<InformacaoRegistro> informacoesRegistro = new List<InformacaoRegistro>();
            string idTransacao = ExtrairInformacao(linha, 27, 49);

            informacoesRegistro.Add(new InformacaoRegistro(registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E01_0")).First().IdRegistro, idArquivo, idTransacao, linha));
            for (int i = 1; i < 5; i++)
            {
                linha = reader.ReadLine();
                informacoesRegistro.Add(new InformacaoRegistro(registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E01_" + TipoRegistroTransacao(linha))).First().IdRegistro, idArquivo, idTransacao, linha));
            }

            Persistir(informacoesRegistro);

        }

        public void TratarRegistroE10(List<Registro> registros,int idArquivo, ref StreamReader reader, string linha)
        {
            List<InformacaoRegistro> informacoesRegistro = new List<InformacaoRegistro>();
            string idTransacao = ExtrairInformacao(linha, 148, 162);

            informacoesRegistro.Add(new InformacaoRegistro(registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E10_0")).First().IdRegistro, idArquivo, idTransacao, linha));
            linha = reader.ReadLine();
            informacoesRegistro.Add(new InformacaoRegistro(registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E10_" + TipoRegistroTransacao(linha))).First().IdRegistro, idArquivo, idTransacao, linha));

            Persistir(informacoesRegistro);
        }



        public void TratarRegistroE20(List<Registro> registros,int idArquivo, ref StreamReader reader, string linha)
        {
            List<InformacaoRegistro> informacoesRegistro = new List<InformacaoRegistro>();
            string idTransacao = ExtrairInformacao(linha, 148, 162);

            informacoesRegistro.Add(new InformacaoRegistro(registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E20_0")).First().IdRegistro, idArquivo, idTransacao, linha));
            linha = reader.ReadLine();
            informacoesRegistro.Add(new InformacaoRegistro(registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E20_" + TipoRegistroTransacao(linha))).First().IdRegistro, idArquivo, idTransacao, linha));

            Persistir(informacoesRegistro);
        }


        public void TratarRegistroE40(List<Registro> registros, int idArquivo, ref StreamReader reader, string linha)
        {
            List<InformacaoRegistro> informacoesRegistro = new List<InformacaoRegistro>();
            string idTransacao = ExtrairInformacao(linha, 28, 50);

            informacoesRegistro.Add(new InformacaoRegistro(registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E40_0")).First().IdRegistro, idArquivo, idTransacao, linha));
            linha = reader.ReadLine();
            informacoesRegistro.Add(new InformacaoRegistro(registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E40_" + TipoRegistroTransacao(linha))).First().IdRegistro, idArquivo, idTransacao, linha));

            Persistir(informacoesRegistro);
        }

        public void Persistir(List<InformacaoRegistro> info)
        {
            
           foreach(InformacaoRegistro i in info)
            {
                InformacaoAux aux = new InformacaoAux() ;
                aux.Chave = i.Chave;
                aux.IdArquivo = i.IdArquivo;
                aux.IdRegistro = i.IdRegistro;
                aux.Valor = StringUtil.Zip(i.Valor);

                buffer.Add(aux);
            }
               
        }

        //public void Finalizar()
        //{
        //    List<InformacaoRegistro> infos;
        //    int indice = 0;
        //    while(buffer.Count > 0)
        //    {
        //        infos =  new List<InformacaoRegistro>();
                
        //        while (infos.Count < 5000 && infos.Count <= buffer.Count)
        //        {   
        //                indice++;
        //                InformacaoRegistro r = new InformacaoRegistro(buffer[indice-1].IdRegistro, buffer[indice-1].IdArquivo, buffer[indice-1].Chave, StringUtil.Unzip(buffer[indice-1].Valor));
        //                infos.Add(r);
                 
        //        }
        //        new InformacaoRegistroDAO().Salvar(infos);
        //        infos.Clear();
        //        buffer.RemoveRange(0,indice);
        //        indice = 0;
        //    }
         
            
        //}


        public void Finalizar()
        {
            //  List<List<InformacaoAux>> partitions = buffer.Select((x, i) => new { Index = i, Value = x })
            //.GroupBy(x => x.Index / 5000)
            //.Select(x => x.Select(v => v.Value).ToList())
            //.ToList();

            List<List<InformacaoAux>> partitions = LAB5Utils.ListUtils<InformacaoAux>.Partition(5000, buffer);
            List<InformacaoRegistro> infos;
           foreach(List<InformacaoAux> laux in partitions)
            {
                infos = new List<InformacaoRegistro>();
                laux.ForEach(l => infos.Add(new InformacaoRegistro(l.IdRegistro, l.IdArquivo, l.Chave, StringUtil.Unzip(l.Valor))));
                new InformacaoRegistroDAO().Salvar(infos);
                infos = null;
            }
        }
        #endregion
    }
}
