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
using CDT.Importacao.Data.Model.Emissores;

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
                    TransacaoElo transElo = new TransacaoElo();
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

        public void SepararCamposTransacao(out TransacaoElo transElo, List<InformacaoRegistro> informacoesRegistro)
        {
            transElo = new TransacaoElo();
            List<Campo> campos = new List<Campo>();
            string tipoTransacao = "", tipoRegistro = "";
            foreach(InformacaoRegistro info in informacoesRegistro)
            {
                tipoRegistro = TipoRegistroTransacao(info.Valor);
                tipoTransacao = TipoTransacaoLinha(info.Valor);
                string chaveRegistro = "REGISTRO_E" + tipoTransacao + "_" + tipoTransacao.PadLeft(2, '0');
                Registro registro = new RegistroDAO().Buscar(chaveRegistro);
                campos.AddRange(registro.Campos);
            }

            //populando objeto transacao
            //transElo.AcquireReferenceNumber = Ex


        }



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

        /*
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
        */

        public void InstanciarObjetoTransacao(out TransacaoElo transacao, Registro registro, string linha)
        {
            List<Campo> campos = registro.Campos.Where(c => c.FlagRelevante == true).ToList();
            transacao = new TransacaoElo();
            switch (TipoTransacaoLinha(linha))
            {
                case Constantes.TE01:
                    switch (TipoRegistroTransacao(linha))
                    {
                        case "0":
                            transacao.CodigoAutorizacao = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("Código da transação")).PosInicio, campos.Find(c => c.NomeCampo.Equals("Código da transação")).PosFim);
                            transacao.Cartao = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("Número do cartão")).PosInicio, campos.Find(c => c.NomeCampo.Equals("Número do cartão")).PosFim);
                            transacao.IdentificacaoTransacao = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("Número de referência da transação")).PosInicio, campos.Find(c => c.NomeCampo.Equals("Número de referência da transação")).PosFim);
                            transacao.DataTransacao = LAB5Utils.DataUtils.RetornaData(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("Número de referência da transação")).PosInicio, campos.Find(c => c.NomeCampo.Equals("Número de referência da transação")).PosFim));
                            transacao.Valor = Decimal.Parse(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("Número de referência da transação")).PosInicio, campos.Find(c => c.NomeCampo.Equals("Número de referência da transação")).PosFim));
                            break;
                        case "1":
                            break;
                        default:
                            break;
                    }
                         
                    break;
            }
        }


        

        public void TratarRegistroE01(List<Registro> registros, int idArquivo, ref StreamReader reader, string linha)
        {
            TransacaoElo transacao;
            List<InformacaoRegistro> informacoesRegistro = new List<InformacaoRegistro>();
            string idTransacao = ExtrairInformacao(linha, 27, 49);
            string _linha = "";
            while((_linha = reader.ReadLine())!= "")
            {
                
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

   


        public void Finalizar()
        {
      
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
