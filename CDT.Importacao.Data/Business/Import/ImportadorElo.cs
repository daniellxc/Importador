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
        RegistroDAO registroDAO = new RegistroDAO();
        List<InformacaoRegistro> buffer = new List<InformacaoRegistro>();
        List<TransacaoElo> bufferElo = new List<TransacaoElo>();
        List<Registro> registrosArquivo = new List<Registro>();


        public bool Importar(Arquivo arquivo)
        {
            
            StreamReader sr;
            int countLinha = 0;
            List<Registro> registros = arquivo.FK_Layout.Registros.ToList();
            List<Informacao> informacoes;
            try {
            
                FileInfo fi = new DirectoryInfo(arquivo.FK_Layout.DiretorioArquivo).GetFiles().Where(f=>f.Name.Equals(arquivo.NomeArquivo)).FirstOrDefault();
                informacoes = new List<Informacao>();
                if (fi != null)
                {
                    sr = new StreamReader(fi.OpenRead());
                    string linha = sr.ReadLine();
                    ImportarInformacaoRegisto(registros.Where(r => r.FK_TipoRegistro.NomeTipoRegistro.Equals("Header")).First(), arquivo.IdArquivo, StringUtil.Zip(linha), "");
                    linha = null;
                    string copia = ""; 
                    while ((linha = sr.ReadLine()) != null)
                    {
                        
                        countLinha++;
                        if (!TipoTransacaoLinha(linha).Equals("BZ"))
                           ProcessarRegistroDetalhe(registros, arquivo, ref sr, linha);

                        copia = linha; 
                    }

                    PersistirBufferInformacoes();
                    ImportarInformacaoRegisto(registros.Where(r => r.FK_TipoRegistro.NomeTipoRegistro.Equals("Trailer")).First(), arquivo.IdArquivo, StringUtil.Zip(copia),"" );
                    return true;
                }
                else
                    throw new IOException("Não existe arquivo com o nome informado");         
            }catch(IOException iox)
            {
                throw new Exception("Erro ao tentar acessar o arquivo." + iox.Message);
            }
            catch(Exception ex)
            {
                throw new Exception("Erro na importação." + ex.Message);
            }
           
        }


        public bool GerarTransacoesEmissor(Arquivo arquivo)
        {
            InformacaoRegistroDAO infregDAO = new InformacaoRegistroDAO();
            RegistroDAO regDAO = new RegistroDAO();
            List<InformacaoRegistro> informacoes = infregDAO.BuscarDetalhesComprimidosArquivo(arquivo.IdArquivo);
            
            int limit = informacoes.Count();

                for (int i = 0; i < limit; i++)
                {
                    try
                    {
                        InformacaoRegistro informacoesTransacao = informacoes[i];
                        if (informacoesTransacao.Chave != string.Empty)
                        {
                            TransacaoElo transacaoElo = new TransacaoElo();
                            transacaoElo.NomeArquivo = arquivo.NomeArquivo;
                            transacaoElo.Id_Incoming = informacoes[i].IdInformacaoRegistro;
                            transacaoElo.FlagTransacaoInternacional = false; //as transacoes internacionais sao processadas em arquivo especifico
                            DecomporLinha(ref transacaoElo, StringUtil.Unzip(informacoesTransacao.Valor));
                            InserirBufferElo(transacaoElo, arquivo.IdEmissor);
                            transacaoElo = null;
                        }
                    
                    }
                    catch(Exception ex)
                    {
                        informacoes[i].FlagErro = true;
                        infregDAO.Update(informacoes[i]);
                        throw new Exception("Erro ao gerar transações na base do emissor." + ex.Message);
                    }
                    
                }

             PersistirBufferElo(arquivo.IdEmissor);
            return true;
            
        }


  

        #region Métodos Auxiliares

    


        private void ImportarInformacaoRegisto(Registro registro, int idArquivo, byte[] linha, string chave)
        {
            new InformacaoRegistroDAO().Salvar(new InformacaoRegistro(registro.IdRegistro, idArquivo ,chave, linha));
        }


        private string ExtrairInformacao(string linha, int ini, int fim)
        {
            try
            {
                return linha.Substring(ini - 1, (fim - ini) + 1);
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Intervalo informado estava fora da linha.");
            }
            catch (Exception)
            {
                return "";
            }
           
           
        }

        private string TipoTransacaoLinha(string linha)
        {
            return ExtrairInformacao(linha, 1, 2);
        }

        private string TipoRegistroTransacao(string linha)
        {
            return ExtrairInformacao(linha, 4, 4);
        }

      

        private void ProcessarRegistroDetalhe(List<Registro> registros,Arquivo arquivo, ref StreamReader reader, string linha)
        {
            string tipoTransacao = TipoTransacaoLinha(linha);

            if (tipoTransacao.Equals(Constantes.TE01))
            {

                TratarRegistroE01(registros, arquivo, ref reader, linha);
            }
            else
            if  ( tipoTransacao.Equals(Constantes.TE05) || tipoTransacao.Equals(Constantes.TE06) || tipoTransacao.Equals(Constantes.TE15) ||
                             tipoTransacao.Equals(Constantes.TE16) || tipoTransacao.Equals(Constantes.TE25) || tipoTransacao.Equals(Constantes.TE26) || tipoTransacao.Equals(Constantes.TE35) ||
                             tipoTransacao.Equals(Constantes.TE36))
            {
               
                TratarRegistroE05(registros, arquivo, ref reader, linha);
              
            }
            else
                        if (tipoTransacao.Equals(Constantes.TE10))
            {
                TratarRegistroE10(registros, arquivo, ref reader, linha);
            }
            else
                        if (tipoTransacao.Equals(Constantes.TE20))
            {
                TratarRegistroE20(registros, arquivo, ref reader, linha);
            }
            else
                        if (tipoTransacao.Equals(Constantes.TE40))
            {

                TratarRegistroE40(registros, arquivo, ref reader, linha);
            }
           
        }

        
       

        public void InstanciarObjetoTransacao(ref TransacaoElo transacao, Registro registro, string linha)
        {
            List<Campo> campos = registro.Campos.Where(c => c.FlagRelevante == true).ToList();
            if (transacao == null)
                transacao = new TransacaoElo();

            string tipoTransacao = TipoTransacaoLinha(linha);

            transacao.CodigoTransacao = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DA TRANSAÇÃO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DA TRANSAÇÃO")).PosFim);
            transacao.TE = tipoTransacao;
           
            

            if (tipoTransacao.Equals(Constantes.TE01) || tipoTransacao.Equals(Constantes.TE05) || tipoTransacao.Equals(Constantes.TE06) || tipoTransacao.Equals(Constantes.TE15) ||
                tipoTransacao.Equals(Constantes.TE16) || tipoTransacao.Equals(Constantes.TE25) || tipoTransacao.Equals(Constantes.TE26) || tipoTransacao.Equals(Constantes.TE35) ||
                tipoTransacao.Equals(Constantes.TE36))
            {

                switch (TipoRegistroTransacao(linha))
                {
                    case "0":
                        
                        transacao.Cartao = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("NÚMERO DO CARTÃO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("NÚMERO DO CARTÃO")).PosFim);
                        transacao.IdentificacaoTransacao = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("NÚMERO DE REFERÊNCIA DA TRANSAÇÃO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("NÚMERO DE REFERÊNCIA DA TRANSAÇÃO")).PosFim);
                        transacao.DataTransacao = LAB5Utils.DataUtils.RetornaData(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("DATA DA VENDA")).PosInicio, campos.Find(c => c.NomeCampo.Equals("DATA DA VENDA")).PosFim));
                        transacao.Valor = Decimal.Parse(StringUtil.StringToMoney(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("VALOR DA VENDA/CHARGEBACK")).PosInicio, campos.Find(c => c.NomeCampo.Equals("VALOR DA VENDA/CHARGEBACK")).PosFim)));
                        transacao.CodigoMoeda = int.Parse(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DE MOEDA DA TRANSAÇÃO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DE MOEDA DA TRANSAÇÃO")).PosFim));
                        transacao.NomeEstabelecimento = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("NOME DO ESTABELECIMENTO COMERCIAL (EC)")).PosInicio, campos.Find(c => c.NomeCampo.Equals("NOME DO ESTABELECIMENTO COMERCIAL (EC)")).PosFim);
                        transacao.CodigoMCC = int.Parse(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DO RAMO DE ATIVIDADE DO EC")).PosInicio, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DO RAMO DE ATIVIDADE DO EC")).PosFim));
                        transacao.Id_CodigoChargeback = int.Parse(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DE MOTIVO DO CHARGEBACK")).PosInicio, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DE MOTIVO DO CHARGEBACK")).PosFim));
                        transacao.CodigoAutorizacao = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DE AUTORIZAÇÃO DA TRANSAÇÃO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DE AUTORIZAÇÃO DA TRANSAÇÃO")).PosFim);
                        transacao.DataProcessamento = LAB5Utils.DataUtils.RetornaData(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("DATA DE MOVIMENTO/APRESENTAÇÃO DO CHARGEBACK")).PosInicio, campos.Find(c => c.NomeCampo.Equals("DATA DE MOVIMENTO/APRESENTAÇÃO DO CHARGEBACK")).PosFim));
                        break;
                    case "1":
                        transacao.IndicadorMeio = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("INDICADOR DE TRANSAÇÃO POR CORRESPONDÊNCIA/TELEFONE/COMÉRCIO ELETRÔNICO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("INDICADOR DE TRANSAÇÃO POR CORRESPONDÊNCIA/TELEFONE/COMÉRCIO ELETRÔNICO")).PosFim);
                        transacao.NumeroParcelas = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("QUANTIDADE DE PARCELAS DA TRANSAÇÃO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("QUANTIDADE DE PARCELAS DA TRANSAÇÃO")).PosFim);
                        transacao.ParcelaPedida = int.Parse(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("NÚMERO DA PARCELA")).PosInicio, campos.Find(c => c.NomeCampo.Equals("NÚMERO DA PARCELA")).PosFim));
                        break;
                    case "2":
                        string tipoOperacao = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("TIPO DE OPERAÇÃO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("TIPO DE OPERAÇÃO")).PosFim);
                        transacao.IdTipoOperacao = tipoOperacao.Trim().Equals(string.Empty) ? 0 : int.Parse(tipoOperacao);
                        transacao.FlagOriginal = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("DATA DE MOVIMENTO DA TRANSAÇÃO ORIGINAL")).PosInicio, campos.Find(c => c.NomeCampo.Equals("DATA DE MOVIMENTO DA TRANSAÇÃO ORIGINAL")).PosFim).Equals("00000000");
                        break;
                    default:
                        break;
                }

            }
            else
                switch(tipoTransacao)
            {
                    case Constantes.TE10:
                        {
                            switch (TipoRegistroTransacao(linha))
                            {
                                case "0":
                                    transacao.Cartao = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("NÚMERO DO CARTÃO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("NÚMERO DO CARTÃO")).PosFim);
                                    transacao.DataTransacao = LAB5Utils.DataUtils.RetornaData(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("DATA DE ENVIO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("DATA DE ENVIO")).PosFim));
                                    transacao.Valor = Decimal.Parse(StringUtil.StringToMoney(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("VALOR DESTINO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("VALOR DESTINO")).PosFim)));
                                    transacao.CodigoMoeda = int.Parse(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("MOEDA DESTINO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("MOEDA DESTINO")).PosFim));
                                    transacao.ValorOrigem = Decimal.Parse(StringUtil.StringToMoney(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("VALOR ORIGEM")).PosInicio, campos.Find(c => c.NomeCampo.Equals("VALOR ORIGEM")).PosFim)));
                                    transacao.CodigoMoedaOrigem = int.Parse(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("MOEDA ORIGEM")).PosInicio, campos.Find(c => c.NomeCampo.Equals("MOEDA ORIGEM")).PosFim));
                                    transacao.CicloApresentacao = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DE MOTIVO DA TRANSAÇÃO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DE MOTIVO DA TRANSAÇÃO")).PosFim);
                                    break;
                                case "2":
                                    transacao.DataProcessamento = LAB5Utils.DataUtils.RetornaData(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("DATA DE PROCESSAMENTO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("DATA DE PROCESSAMENTO")).PosFim));
                                    break;
                            }

                            break;
                        }

                    case Constantes.TE20:
                        {
                            switch (TipoRegistroTransacao(linha))
                            {
                                case "0":
                                    transacao.Cartao = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("NÚMERO DO CARTÃO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("NÚMERO DO CARTÃO")).PosFim);
                                    transacao.DataTransacao = LAB5Utils.DataUtils.RetornaData(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("DATA DE ENVIO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("DATA DE ENVIO")).PosFim));
                                    transacao.Valor = Decimal.Parse(StringUtil.StringToMoney(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("VALOR DESTINO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("VALOR DESTINO")).PosFim)));
                                    transacao.CodigoMoeda = int.Parse(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("MOEDA DESTINO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("MOEDA DESTINO")).PosFim));
                                    transacao.ValorOrigem = Decimal.Parse(StringUtil.StringToMoney(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("VALOR ORIGEM")).PosInicio, campos.Find(c => c.NomeCampo.Equals("VALOR ORIGEM")).PosFim)));
                                    transacao.CodigoMoedaOrigem = int.Parse(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("MOEDA ORIGEM")).PosInicio, campos.Find(c => c.NomeCampo.Equals("MOEDA ORIGEM")).PosFim));
                                    transacao.CicloApresentacao = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DE MOTIVO DA TRANSAÇÃO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DE MOTIVO DA TRANSAÇÃO")).PosFim);
                                    break;
                                case "2":
                                    transacao.DataProcessamento = LAB5Utils.DataUtils.RetornaData(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("DATA DE PROCESSAMENTO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("DATA DE PROCESSAMENTO")).PosFim));
                                    break;
                            }
                            break;
                        }
                    case Constantes.TE40:
                        {
                            switch (TipoRegistroTransacao(linha))
                            {
                                case "0":
                                    transacao.Cartao = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("NÚMERO DO CARTÃO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("NÚMERO DO CARTÃO")).PosFim);
                                    transacao.IdentificacaoTransacao = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("NÚMERO DE REFERÊNCIA DA TRANSAÇÃO")).PosInicio, campos.Find(c => c.NomeCampo.Equals("NÚMERO DE REFERÊNCIA DA TRANSAÇÃO")).PosFim);
                                    transacao.DataTransacao = LAB5Utils.DataUtils.RetornaData(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("DATA DA VENDA")).PosInicio, campos.Find(c => c.NomeCampo.Equals("DATA DA VENDA")).PosFim));
                                    transacao.NomeEstabelecimento = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("NOME DO EC")).PosInicio, campos.Find(c => c.NomeCampo.Equals("NOME DO EC")).PosFim);
                                    transacao.CodigoMCC = int.Parse(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DO RAMO DE ATIVIDADE DO EC")).PosInicio, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DO RAMO DE ATIVIDADE DO EC")).PosFim));
                                    transacao.Valor = Decimal.Parse(StringUtil.StringToMoney(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("VALOR DA FRAUDE")).PosInicio, campos.Find(c => c.NomeCampo.Equals("VALOR DA FRAUDE")).PosFim)));
                                    transacao.CodigoMoeda = int.Parse(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DE MOEDA DA TRANSAÇÃO FRAUDULENTA")).PosInicio, campos.Find(c => c.NomeCampo.Equals("CÓDIGO DE MOEDA DA TRANSAÇÃO FRAUDULENTA")).PosFim));
                                    transacao.DataProcessamento = LAB5Utils.DataUtils.RetornaData(ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("DATA DE NOTIFICAÇÃO DA FRAUDE")).PosInicio, campos.Find(c => c.NomeCampo.Equals("DATA DE NOTIFICAÇÃO DA FRAUDE")).PosFim));
                                    transacao.CicloApresentacao = ExtrairInformacao(linha, campos.Find(c => c.NomeCampo.Equals("TIPO DE FRAUDE")).PosInicio, campos.Find(c => c.NomeCampo.Equals("TIPO DE FRAUDE")).PosFim);
                                    break;
                                case "2":
                                    break;
                            }
                            break; 
                        }
               
            }
           
            
            campos = null;
        }


        public void TratarRegistroE01(List<Registro> registros, Arquivo arquivo, ref StreamReader reader, string linha)
        {
            string idTransacao = ExtrairInformacao(linha, 27, 49);
            string linhaResult = registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E01_0")).First().IdRegistro.ToString() + ComporLinha(linha);
            for (int i = 1; i < 6; i++)
            {
                linha = reader.ReadLine();
                linhaResult += registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E01_" + TipoRegistroTransacao(linha))).First().IdRegistro.ToString() + ComporLinha(linha);

            }

            RegistrarInformacaoNoBuffer(new InformacaoRegistro { Chave = idTransacao, IdArquivo = arquivo.IdArquivo, Valor = StringUtil.Zip(linhaResult) });


        }


        public void TratarRegistroE05(List<Registro> registros, Arquivo arquivo, ref StreamReader reader, string linha)
         {
             
             string idTransacao = ExtrairInformacao(linha, 27, 49);
             string linhaResult = registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E01_0")).First().IdRegistro.ToString() + ComporLinha(linha);
             for (int i = 1; i < 5; i++)
             {
                linha = reader.ReadLine();
                linhaResult += registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E01_" + TipoRegistroTransacao(linha))).First().IdRegistro.ToString() + ComporLinha(linha);
               
             }

            RegistrarInformacaoNoBuffer(new InformacaoRegistro { Chave = idTransacao, IdArquivo = arquivo.IdArquivo, Valor = StringUtil.Zip(linhaResult)});
         }

      

        public void TratarRegistroE10(List<Registro> registros,Arquivo arquivo, ref StreamReader reader, string linha)
        {
          
            string idTransacao = ExtrairInformacao(linha, 148, 162);
            string linhaResult = registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E10_0")).First().IdRegistro.ToString() + ComporLinha(linha);
            
            linha = reader.ReadLine();
            linhaResult += registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E10_" + TipoRegistroTransacao(linha))).First().IdRegistro.ToString() + ComporLinha(linha);
            RegistrarInformacaoNoBuffer(new InformacaoRegistro { Chave = idTransacao, IdArquivo = arquivo.IdArquivo, Valor = StringUtil.Zip(linhaResult) });
        }



        public void TratarRegistroE20(List<Registro> registros, Arquivo arquivo, ref StreamReader reader, string linha)
        {
            string idTransacao = ExtrairInformacao(linha, 148, 162);
            string linhaResult = registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E20_0")).First().IdRegistro.ToString() + ComporLinha(linha);

            linha = reader.ReadLine();
            linhaResult += registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E20_" + TipoRegistroTransacao(linha))).First().IdRegistro.ToString() + ComporLinha(linha);
            RegistrarInformacaoNoBuffer(new InformacaoRegistro { Chave = idTransacao, IdArquivo = arquivo.IdArquivo, Valor = StringUtil.Zip(linhaResult) });
        }


        public void TratarRegistroE40(List<Registro> registros, Arquivo arquivo, ref StreamReader reader, string linha)
        {
            string idTransacao = ExtrairInformacao(linha, 148, 162);
            string linhaResult = registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E40_0")).First().IdRegistro.ToString() + ComporLinha(linha);

            linha = reader.ReadLine();
            linhaResult += registros.Where(r => r.ChaveRegistro.Equals("REGISTRO_E40_" + TipoRegistroTransacao(linha))).First().IdRegistro.ToString() + ComporLinha(linha);
            RegistrarInformacaoNoBuffer(new InformacaoRegistro { Chave = idTransacao, IdArquivo = arquivo.IdArquivo, Valor = StringUtil.Zip(linhaResult) });
        }
   
        public void RegistrarInformacaoNoBuffer(InformacaoRegistro info)
        {
           
            buffer.Add(info);
        }

        private string ComporLinha(string linha)
        {
            return Constantes.SPLITTER_REGISTRO + linha + Constantes.SPLITTER_LINHA;
        }

        public void DecomporLinha(ref TransacaoElo transacaoElo, string linha)
        {

            string[] linhasBase = StringUtil.Split(Constantes.SPLITTER_LINHA, linha);
            foreach (string _linha in linhasBase)
            {
                string[] linhaComposta = StringUtil.Split(Constantes.SPLITTER_REGISTRO, _linha);
                //InstanciarObjetoTransacao(ref transacaoElo, registrosArquivo.Where(x => x.IdRegistro == (int.Parse(linhaComposta[0]))).First(), linhaComposta[1]);
                InstanciarObjetoTransacao(ref transacaoElo, registroDAO.Buscar(int.Parse(linhaComposta[0])), linhaComposta[1]);
            }

        }

        private void InserirBufferElo(TransacaoElo transacao, int idEmissor)
        {
            if (bufferElo.Count < Constantes.BUFFER_LIMIT)
                bufferElo.Add(transacao);
            else
            {
                PersistirBufferElo(idEmissor);
                bufferElo.Add(transacao);
            }

        }

        private void PersistirBufferElo(int idEmissor)
        {
            if (bufferElo.Count > 0)
            {
                new TransacoesEloDAO(idEmissor).Salvar(bufferElo);
                bufferElo.Clear();
            }

        }



        public void PersistirBufferInformacoes()
        {
      
            List<List<InformacaoRegistro>> partitions = LAB5Utils.ListUtils<InformacaoRegistro>.Partition(5000, buffer);
            List<InformacaoRegistro> infos;
           foreach(List<InformacaoRegistro> laux in partitions)
            {
                infos = new List<InformacaoRegistro>();
                laux.ForEach(l => infos.Add(new InformacaoRegistro(l.IdRegistro, l.IdArquivo, l.Chave, l.Valor)));
                new InformacaoRegistroDAO().Salvar(infos);
                infos = null;
            }
        }

        public void AtualizarBufferInformacoes()
        {

            List<List<InformacaoRegistro>> partitions = LAB5Utils.ListUtils<InformacaoRegistro>.Partition(5000, buffer);
            List<InformacaoRegistro> infos;
            foreach (List<InformacaoRegistro> laux in partitions)
            {
                infos = new List<InformacaoRegistro>();
                laux.ForEach(l => infos.Add(new InformacaoRegistro(l.IdRegistro, l.IdArquivo, l.Chave, l.Valor,l.FlagErro)));
                new InformacaoRegistroDAO().Update(infos);
                infos = null;
            }
        }

        public void MontarArquivoRetorno(int idArquivo)
        {

            Arquivo incoming = new ArquivoDAO().Buscar(idArquivo);
            InformacaoRegistro headerIncoming, trailerIncoming, detailOutgoing;
            Registro regHeader = new RegistroDAO().RegistroPorArquivo(idArquivo).Where(r => r.FK_TipoRegistro.NomeTipoRegistro.ToLower().Equals("header")).First(); 
            headerIncoming  = infRegistroDAO.BuscarHeaderArquivo(idArquivo);
            trailerIncoming = infRegistroDAO.BuscarTrailerArquivo(idArquivo);
            string detail = new string('0',168);

            string headerReal = LAB5Utils.StringUtils.Unzip(headerIncoming.Valor);

            headerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(headerReal, LAB5Utils.DataUtils.RetornaDataYYYYMMDD(DateTime.Now), regHeader.Campos.Where(c => c.NomeCampo.Equals("DATA DE RETORNO DO ARQUIVO")).First().PosInicio, regHeader.Campos.Where(c => c.NomeCampo.Equals("DATA DE RETORNO DO ARQUIVO")).First().PosFim);
            headerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(headerReal, LAB5Utils.DataUtils.RetornaHoraHHMMSS(DateTime.Now), regHeader.Campos.Where(c => c.NomeCampo.Equals("HORA DE RETORNO DO ARQUIVO")).First().PosInicio, regHeader.Campos.Where(c => c.NomeCampo.Equals("HORA DE RETORNO DO ARQUIVO")).First().PosFim);
            headerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(headerReal, "2", regHeader.Campos.Where(c => c.NomeCampo.Equals("INDICADOR DE ROTA DO ARQUIVO")).First().PosInicio, regHeader.Campos.Where(c => c.NomeCampo.Equals("INDICADOR DE ROTA DO ARQUIVO")).First().PosFim);

            headerIncoming.Valor = LAB5Utils.StringUtils.Zip(headerReal);

            infRegistroDAO.Salvar(headerIncoming);

            List<Campo> camposDetail = new RegistroDAO().RegistroPorArquivo(idArquivo).Where(r => r.ChaveRegistro.Equals("REGISTRO_E44_0")).First().Campos.OrderBy(c=>c.PosInicio).ToList();

            foreach(Campo campo in camposDetail)
            {

            }



        }

        public void MontarInformacao(Campo campo, string valor, ref string destino)
        {
            LAB5Utils.ArquivoUtils.AlterarInformacao(destino, valor, campo.PosInicio, campo.PosFim);
        }

        #endregion
    }
}
