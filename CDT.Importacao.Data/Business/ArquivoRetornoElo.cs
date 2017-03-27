using CDT.Importacao.Data.Business.Import;
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using CDT.Importacao.Data.Model.Emissores;
using CDT.Importacao.Data.Utils;
using LAB5;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Business
{
    /// <summary>
    /// Classe para manipulação do arquivo de retorno da liquidação nacional ELO
    /// </summary>
    public class ArquivoRetornoElo
    {
        #region Variáveis utilizadas em toda classe
        private InformacaoRegistroDAO infRegDAO = null;
        private TransacoesEloDAO transacoesEloDAO = null;
        private Arquivo arquivo;
        private int totalRegistros = 0, numRemessa = 0;
        private string dataRemessa,  situacaoRemessa, motivoRejeicaoRemessa;
        #endregion


        public ArquivoRetornoElo(Arquivo arquivo)
        {
            transacoesEloDAO = new TransacoesEloDAO(arquivo.IdEmissor);
            infRegDAO = new InformacaoRegistroDAO();

            this.arquivo = arquivo;
            totalRegistros = infRegDAO.TransacoesAceitas(arquivo.IdArquivo).Count;
            numRemessa = new ImportadorElo().NumeroRemessaRecebida(arquivo.IdArquivo);
        }

        
        public bool PodeGerar()
        {
            return transacoesEloDAO.TransacoesProcessadas(arquivo.NomeArquivo).Count > 0;
        }


        private string MontarHeader()
        {
            string situacao = "";

            InformacaoRegistro headerIncoming;
            Registro regHeader = new RegistroDAO().RegistroPorArquivo(arquivo.IdArquivo).Where(r => r.FK_TipoRegistro.NomeTipoRegistro.ToLower().Equals("header")).First();
            headerIncoming = infRegDAO.BuscarHeaderArquivo(arquivo.IdArquivo);
            if (headerIncoming.Equals(null)) throw new Exception("Header não localizado");
            headerIncoming.IdInformacaoRegistro = 0;
            headerIncoming.IdRegistro = new RegistroDAO().Buscar("B0-OUTGOING", arquivo.IdLayout).IdRegistro;
            string headerReal = LAB5Utils.StringUtils.Unzip(headerIncoming.Valor);
            dataRemessa = LAB5Utils.ArquivoUtils.ExtrairInformacao(headerReal, 5, 12);

            situacao = ValidarRemessaRecebidaAdquirencia(headerReal);
            situacaoRemessa = situacao != "" ? "R" : "A";
            motivoRejeicaoRemessa = situacao != "" ? situacao : "";
            //if (situacaoRemessa == "A")
            //    numRemessa++;



            //headerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(headerReal, numRemessa.ToString().PadLeft(4, '0'), 13, 16);
            headerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(headerReal, LAB5Utils.DataUtils.RetornaDataYYYYMMDD(DateTime.Now), regHeader.Campos.Where(c => c.NomeCampo.Equals("DATA DE RETORNO DO ARQUIVO")).First().PosInicio, regHeader.Campos.Where(c => c.NomeCampo.Equals("DATA DE RETORNO DO ARQUIVO")).First().PosFim);
            headerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(headerReal, LAB5Utils.DataUtils.RetornaHoraHHMMSS(DateTime.Now), regHeader.Campos.Where(c => c.NomeCampo.Equals("HORA DE RETORNO DO ARQUIVO")).First().PosInicio, regHeader.Campos.Where(c => c.NomeCampo.Equals("HORA DE RETORNO DO ARQUIVO")).First().PosFim);
            headerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(headerReal, "2", regHeader.Campos.Where(c => c.NomeCampo.Equals("INDICADOR DE ROTA DO ARQUIVO")).First().PosInicio, regHeader.Campos.Where(c => c.NomeCampo.Equals("INDICADOR DE ROTA DO ARQUIVO")).First().PosFim);

            headerIncoming.Valor = LAB5Utils.StringUtils.Zip(headerReal);
            try
            {
                infRegDAO.Salvar(headerIncoming);
                return LAB5Utils.StringUtils.Unzip(headerIncoming.Valor);

            }catch(Exception ex)
            {
                throw ex;
            }
            
        }

        private string MontarRegistroTE44()
        {
            int totalTransacoesEmReal = transacoesEloDAO.TransacoesConciliadasPorCodigoMoeda(arquivo.NomeArquivo, 986).Count;
            decimal valorTransacoesEmReal = transacoesEloDAO.TransacoesConciliadasPorCodigoMoeda(arquivo.NomeArquivo, 986).Sum(x => x.Valor);
            int totalTransacoesNaoAceitasEmReal = transacoesEloDAO.TransacoesNaoConciliadasPorCodigoMoeda(arquivo.NomeArquivo, 986).Count;
            decimal valorTransacoesNaoAceitasEmReal = transacoesEloDAO.TransacoesNaoConciliadasPorCodigoMoeda(arquivo.NomeArquivo, 986).Sum(x => x.Valor);

            int totalTransacoesEmDolar = transacoesEloDAO.TransacoesConciliadasPorCodigoMoeda(arquivo.NomeArquivo, 840).Count;
            decimal valorTransacoesEmDolar = transacoesEloDAO.TransacoesConciliadasPorCodigoMoeda(arquivo.NomeArquivo, 840).Sum(x => x.Valor);
            int totalTransacoesNaoAceitasEmDolar = transacoesEloDAO.TransacoesNaoConciliadasPorCodigoMoeda(arquivo.NomeArquivo, 840).Count;
            decimal valorTransacoesNaoAceitasEmDolar = transacoesEloDAO.TransacoesNaoConciliadasPorCodigoMoeda(arquivo.NomeArquivo, 840).Sum(x => x.Valor);

            string valorDetail = new string('0', 168);

            try
            {

                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, "44", 1, 2);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, "00", 3, 4);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, arquivo.FK_Emissor.CodigoEmissorFebraban, 5, 9);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, string.Empty.PadLeft((13-10) + 1, ' '), 10, 13);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, dataRemessa, 14, 21);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, numRemessa.ToString().PadLeft((27-22) +1 , '0'), 22, 27);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, LAB5Utils.DataUtils.RetornaDataYYYYMMDD(DateTime.Now), 28, 35);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, situacaoRemessa, 36, 36);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, motivoRejeicaoRemessa.PadLeft((38-37) + 1,' '), 37, 38);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, "986", 39, 41);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, string.Empty.ToString().PadLeft((42-42) + 1,' '), 42, 42);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, totalRegistros.ToString().PadLeft((57-43) + 1, '0'), 43, 57);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, totalTransacoesEmReal.ToString().PadLeft((72-58)+1, '0'), 58, 72);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, valorTransacoesEmReal.ToString().Replace(',', ' ').Replace('.', ' ').ToString().PadLeft((87-73) +1, '0'), 73, 87);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, totalTransacoesNaoAceitasEmReal.ToString().PadLeft((95 - 88) + 1, '0'), 88, 95);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, valorTransacoesNaoAceitasEmReal.ToString().Replace(',', ' ').Replace('.', ' ').PadLeft((110 - 96) + 1, '0'), 96, 110);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, "007", 111, 113);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, totalTransacoesEmDolar.ToString().PadLeft((121 - 114) + 1, '0'), 114, 121);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, valorTransacoesEmDolar.ToString().Replace(',', ' ').Replace('.', ' ').ToString().PadLeft((136 - 122) + 1, '0'), 122, 136);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, totalTransacoesNaoAceitasEmDolar.ToString().PadLeft((144 - 137) + 1, '0'), 137, 144);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, valorTransacoesNaoAceitasEmDolar.ToString().Replace(',', ' ').Replace('.', ' ').PadLeft((159 - 145) + 1, '0'), 145, 159);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, string.Empty.PadLeft((168 - 160) + 1, ' '), 160, 168);

                int idRegistroDetail = new RegistroDAO().RegistroPorArquivo(arquivo.IdArquivo).Where(r => r.ChaveRegistro.Equals("REGISTRO_E44_0")).First().IdRegistro;
                InformacaoRegistro TE44 = new InformacaoRegistro(idRegistroDetail, arquivo.IdArquivo, "0", LAB5Utils.StringUtils.Zip(valorDetail), false);

                infRegDAO.Salvar(TE44);

                return valorDetail;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar detail para o arquivo retorno." + ex.Message + " - " + ex.InnerException.Message);
            }
        }

        private List<string> MontarRegistrosTE05Rejeitados()
        {
            List<string> retorno = new List<string>();
            List<InformacaoRegistro> registrosRejeitados = infRegDAO.ListarRegistrosRejeitados(arquivo.IdArquivo);
            if(registrosRejeitados != null)
            {
                foreach (InformacaoRegistro info in registrosRejeitados)
                {
                    //decompor as linhas dos registros 0,1 e 2
                    retorno.AddRange(QuebrarLinhasEmRegistros(LAB5Utils.StringUtils.Unzip(info.Valor)));
                    //adcionar registro 9
                    retorno.Add(MontarRegistroTE09(info));

                }
            }
           
            return retorno;
        }

        private string MontarRegistroTE09(InformacaoRegistro info)
        {
            
            List<string> registros = QuebrarLinhasEmRegistros(LAB5Utils.StringUtils.Unzip(info.Valor));
            string registroTE09 = new string('0', 168);
            string dataMovimentoOriginal = LAB5Utils.ArquivoUtils.ExtrairInformacao(registros[0],161,168);

            registroTE09 = LAB5Utils.ArquivoUtils.AlterarInformacao(registroTE09, "0109", 1, 4);
            registroTE09 = LAB5Utils.ArquivoUtils.AlterarInformacao(registroTE09, string.Empty.PadLeft((16-5) + 1,' '), 5, 16);
            registroTE09 = LAB5Utils.ArquivoUtils.AlterarInformacao(registroTE09, "05", 17, 18);
            //registroTE09 = LAB5Utils.ArquivoUtils.AlterarInformacao(registroTE09, "00", 19, 20);
            registroTE09 = LAB5Utils.ArquivoUtils.AlterarInformacao(registroTE09,dataMovimentoOriginal, 21, 28);
            registroTE09 = LAB5Utils.ArquivoUtils.AlterarInformacao(registroTE09, string.Empty.PadLeft((35 - 29) + 1, ' '), 29, 35);
            registroTE09 = LAB5Utils.ArquivoUtils.AlterarInformacao(registroTE09, info.Erro, 36, 38);
            registroTE09 = LAB5Utils.ArquivoUtils.AlterarInformacao(registroTE09, string.Empty.PadLeft((168 - 39) + 1, ' '),39, 168);

            return registroTE09;


        }

        private List<string> MontarDetail()
        {
            List<string> details = new List<string>();
            try
            {
                if (numRemessa.Equals(null) || dataRemessa.Equals(null))
                    throw new Exception("Não é possível iniciar detail. Header não foi montado");

                details.Add(MontarRegistroTE44());
                details.AddRange(MontarRegistrosTE05Rejeitados());

            }
            catch
            {
                throw;
            }
            
            return details;   
        }

        private string MontarTrailer()
        {
            TransacoesEloDAO transacaoDAO = new TransacoesEloDAO(arquivo.IdEmissor);
            InformacaoRegistro trailerIncoming = infRegDAO.BuscarTrailerArquivo(arquivo.IdArquivo);
            if (trailerIncoming.Equals(null)) throw new Exception("Trailer não localizado");
            trailerIncoming.IdInformacaoRegistro = 0;
            int totalTransacoesCredReal, totalTransacoesCredDolar, totalTransacoesDebReal, totalTransacoesDebDolar;
            decimal valorTransacaoesCredReal, valorTransacoesCredDolar, valorTransacoesDebReal, valorTransacoesDedDolar;

            string trailerReal = LAB5Utils.StringUtils.Unzip(trailerIncoming.Valor);

            string strValorCredReal, strValorCredDolar, strValorDebReal, strValorDebDolar;

            
            

            totalTransacoesCredReal = transacaoDAO.TransacoesCredito(arquivo.NomeArquivo).Where(t=>t.CodigoMoeda == 986).ToList().Count;
            valorTransacaoesCredReal = Math.Round(transacaoDAO.TransacoesCredito(arquivo.NomeArquivo).Where(t => t.CodigoMoeda == 986).ToList().Sum(t => t.Valor),2);
            totalTransacoesCredDolar = transacaoDAO.TransacoesCredito(arquivo.NomeArquivo).Where(t => t.CodigoMoeda == 840).ToList().Count;
            valorTransacoesCredDolar = Math.Round(transacaoDAO.TransacoesCredito(arquivo.NomeArquivo).Where(t => t.CodigoMoeda == 840).ToList().Sum(t => t.Valor),2);
            totalTransacoesDebReal = transacaoDAO.TransacoesDebito(arquivo.NomeArquivo).Where(t => t.CodigoMoeda == 986).ToList().Count;
            totalTransacoesDebDolar = transacaoDAO.TransacoesDebito(arquivo.NomeArquivo).Where(t => t.CodigoMoeda == 840).ToList().Count;
            valorTransacoesDebReal =  Math.Round(transacaoDAO.TransacoesDebito(arquivo.NomeArquivo).Where(t => t.CodigoMoeda == 986).Sum(t => t.Valor),2);
            valorTransacoesDedDolar = Math.Round(transacaoDAO.TransacoesDebito(arquivo.NomeArquivo).Where(t => t.CodigoMoeda == 840).Sum(t => t.Valor),2);

            strValorCredDolar = valorTransacoesCredDolar > 0 ? valorTransacoesCredDolar.ToString().Remove(valorTransacoesCredDolar.ToString().IndexOf(','), 1).PadLeft(15, '0') : new string('0',15);
            strValorCredReal = valorTransacaoesCredReal > 0 ? valorTransacaoesCredReal.ToString().Remove(valorTransacaoesCredReal.ToString().IndexOf(','), 1).PadLeft(15, '0') : new string('0', 15);
            strValorDebReal = valorTransacoesDebReal > 0 ? valorTransacoesDebReal.ToString().Remove(valorTransacoesDebReal.ToString().IndexOf(','), 1).PadLeft(15, '0') : new string('0', 15);
            strValorDebDolar = valorTransacoesDedDolar > 0 ? valorTransacoesDedDolar.ToString().Remove(valorTransacoesDedDolar.ToString().IndexOf(','), 1).PadLeft(15, '0') : new string('0', 15);
                                                                
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, totalTransacoesCredReal.ToString().PadLeft(8, '0'), 9, 16);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal,strValorCredReal , 17, 31);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, totalTransacoesDebReal.ToString().PadLeft(8, '0'), 32, 39);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, strValorDebReal, 40, 54);
            
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, totalTransacoesCredDolar.ToString().PadLeft(8, '0'), 55, 62);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, strValorCredDolar, 63, 77);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, totalTransacoesDebDolar.ToString().PadLeft(8, '0'), 78, 85);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, strValorDebDolar, 86, 100);

            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal,totalRegistros.ToString().PadLeft(8,'0'),101,108);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, new string(' ', (116-109) + 1), 109, 116);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, new string(' ', (131-117) + 1), 117, 131);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, new string(' ', (167-132) + 1), 132, 167);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal,"2", 168, 168);

            trailerIncoming.Valor = LAB5Utils.StringUtils.Zip(trailerReal);

            try
            {
                infRegDAO.Salvar(trailerIncoming);
                return LAB5Utils.StringUtils.Unzip(trailerIncoming.Valor);
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível gerar o trailer do arquivo de retorno. " + ex.Message + " - " + ex.InnerException);
            }

        }

        public bool MontarArquivoRetorno(string diretorioDestino,string nomeArquivo)
        {
           
                StreamWriter sw;
                string header, trailer;
                List<string> details;

                if (!Directory.Exists(diretorioDestino))
                {
                    throw new Exception("O Diretório de destino do arquivo não existe");
                }



                try
                {

                    header = MontarHeader();
                    details = MontarDetail();
                    trailer = MontarTrailer();
                    ValidarEstruturaArquivo(header, details[0], trailer);
                    sw = new StreamWriter(diretorioDestino + "\\" + nomeArquivo);
                    sw.WriteLine(header);
                    header = null;
                    //se a remesse for aceita ou parcialmente aceita, escreve o registro TE44 e as transacoes rejeitas
                    if (situacaoRemessa == "A")
                        {
                            foreach (string str in details)
                                sw.WriteLine(str);
                        }
                    //senao escreve apenas o registro TE44
                    else
                        sw.WriteLine(details[0]);
                    details = null;
                    sw.WriteLine(trailer);
                    sw.Flush();
                    sw.Close();
                    trailer = null;
                    return true;

                }
                catch (Exception ex)
                {
                    OnErro();
                    throw new Exception("Erro ao montar o arquivo. " + ex.Message);
                }


    


        }

        private void AtualizarNumeroRemessa(int numRemessa, ref string header, ref string TE44)
        {
            LAB5Utils.ArquivoUtils.AlterarInformacao(header, numRemessa.ToString().PadLeft(4, '0'), 13, 16);
            LAB5Utils.ArquivoUtils.AlterarInformacao(TE44, numRemessa.ToString().PadLeft(6, '0'), 22, 27);
        }

        private string ValidarRemessaRecebidaAdquirencia(string header)
        {
            ImportadorElo importador = new ImportadorElo();
            int numRec = importador.NumeroRemessaRecebida(arquivo.IdArquivo);
            int numEnv = importador.NumeroRemessaEnviada(arquivo.IdArquivo);

           //if ((numEnv > 0) && (numEnv  - numRec) != 1)
           //     return "02";//NUMERO DE REMESSA FORA DE SEQUENCIA
            if (LAB5Utils.ArquivoUtils.ExtrairInformacao(header, 3, 4) != "10")
                return "11";//CODIGO DE SERVICO NAO PREVISTO
            if (LAB5Utils.ArquivoUtils.ExtrairInformacao(header, 49, 52) != "0063")
                return "13";//CODIGO DO EMISSOR INVALIDO
            if (LAB5Utils.ArquivoUtils.ExtrairInformacao(header, 165, 167) != "007")
                return "13";//CODIGO DE BANDEIRA INVALIDO
            return "";
        }

        private void ValidarEstruturaArquivo(string header, string TE44, string trailer)
        {
            if (header != null && TE44 != null && trailer != null)
            {
                

                if (!situacaoRemessa.ToUpper().Equals("A") && !situacaoRemessa.ToUpper().Equals("R"))
                    throw new Exception("Valor inválido para situação da remessa. Valores permitidos: A e R");

                if (LAB5Utils.ArquivoUtils.ExtrairInformacao(header, 5, 12) != LAB5Utils.ArquivoUtils.ExtrairInformacao(TE44, 14, 21))
                    throw new Exception("Data de remessa difere no header e no registro T44-0");
                if (int.Parse(LAB5Utils.ArquivoUtils.ExtrairInformacao(header, 13, 16)) != int.Parse(LAB5Utils.ArquivoUtils.ExtrairInformacao(TE44, 22, 27)))
                    throw new Exception("Número de remessa difere no header e no registro T44-0");

            }
            else
                throw new Exception("Estrutura do arquivo incorreta.");
        }

        private List<string> QuebrarLinhasEmRegistros(string linha)
        {
            List<string> retorno = new List<string>();
            string[] linhasRegistros = LAB5Utils.StringUtils.Split(Constantes.SPLITTER_LINHA, linha);
            //APENAS OS REGISTROS 0,1 e 2, SEGUNDO O MANUAL DE LIQUIDACAO ELO
            for (int i = 0; i<= 2; i++)
            {
                string registrosReal = LAB5Utils.StringUtils.Split(Constantes.SPLITTER_REGISTRO, linhasRegistros[i])[1];
                registrosReal =  LAB5Utils.ArquivoUtils.AlterarInformacao(registrosReal, "01", 1, 2);
                retorno.Add(registrosReal);
                
            }
           
            return retorno;
        }

   


        public void OnErro()
        {
            List<Registro> registrosRetorno = new RegistroDAO().Listar(x => x.ChaveRegistro == "B0-OUTGOING" || x.ChaveRegistro.Contains("E44"));
            new InformacaoRegistroDAO().Delete(registrosRetorno);
        }
     


    }
}
