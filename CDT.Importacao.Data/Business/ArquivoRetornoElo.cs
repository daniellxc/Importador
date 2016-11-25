using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using CDT.Importacao.Data.Model.Emissores;
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
             private InformacaoRegistroDAO infRegDAO = new InformacaoRegistroDAO();
             private Arquivo arquivo;
             private int totalRegistros = 0;
             private string dataRemessa, numRemessa;
        #endregion


        public ArquivoRetornoElo(Arquivo arquivo)
        {
            this.arquivo = arquivo;
            totalRegistros = infRegDAO.TransacoesAceitas(arquivo.IdArquivo).Count + 2; //total + header + trailer 
        }



        private InformacaoRegistro MontarHeader()
        {

            InformacaoRegistro headerIncoming;
            Registro regHeader = new RegistroDAO().RegistroPorArquivo(arquivo.IdArquivo).Where(r => r.FK_TipoRegistro.NomeTipoRegistro.ToLower().Equals("header")).First();
            headerIncoming = infRegDAO.BuscarHeaderArquivo(arquivo.IdArquivo);
            if (headerIncoming.Equals(null)) throw new Exception("Header não localizado");
            headerIncoming.IdInformacaoRegistro = 0;
            string headerReal = LAB5Utils.StringUtils.Unzip(headerIncoming.Valor);

            dataRemessa = LAB5Utils.ArquivoUtils.ExtrairInformacao(headerReal, 5, 12);
            numRemessa = LAB5Utils.ArquivoUtils.ExtrairInformacao(headerReal, 13, 16);

            headerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(headerReal, LAB5Utils.DataUtils.RetornaDataYYYYMMDD(DateTime.Now), regHeader.Campos.Where(c => c.NomeCampo.Equals("DATA DE RETORNO DO ARQUIVO")).First().PosInicio, regHeader.Campos.Where(c => c.NomeCampo.Equals("DATA DE RETORNO DO ARQUIVO")).First().PosFim);
            headerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(headerReal, LAB5Utils.DataUtils.RetornaHoraHHMMSS(DateTime.Now), regHeader.Campos.Where(c => c.NomeCampo.Equals("HORA DE RETORNO DO ARQUIVO")).First().PosInicio, regHeader.Campos.Where(c => c.NomeCampo.Equals("HORA DE RETORNO DO ARQUIVO")).First().PosFim);
            headerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(headerReal, "2", regHeader.Campos.Where(c => c.NomeCampo.Equals("INDICADOR DE ROTA DO ARQUIVO")).First().PosInicio, regHeader.Campos.Where(c => c.NomeCampo.Equals("INDICADOR DE ROTA DO ARQUIVO")).First().PosFim);

            headerIncoming.Valor = LAB5Utils.StringUtils.Zip(headerReal);
            try
            {
                infRegDAO.Salvar(headerIncoming);
                return headerIncoming;

            }catch(Exception ex)
            {
                throw ex;
            }
            
        }

        private InformacaoRegistro MontarDetail(string dataRemessa, string numRemessa, string situacao, string motivo)
        {
            if (numRemessa.Equals(null) || dataRemessa.Equals(null))
                throw new Exception("Não é possível iniciar detail. Header não foi montado");

            TransacoesEloDAO transacoesDAO = new TransacoesEloDAO(arquivo.IdEmissor);
            

            int totalTrnsacoesEmReal = transacoesDAO.TransacoesPorCodigoMoeda(arquivo.NomeArquivo, 986).Count;
            decimal valorTransacoesEmReal = transacoesDAO.TransacoesPorCodigoMoeda(arquivo.NomeArquivo, 986).Sum(x => x.Valor);

            int totalTransacoesEmDolar = transacoesDAO.TransacoesPorCodigoMoeda(arquivo.NomeArquivo, 840).Count;
            decimal valorTransacoesEmDolar = transacoesDAO.TransacoesPorCodigoMoeda(arquivo.NomeArquivo, 840).Sum(x => x.Valor);

            string valorDetail = new string('0', 168);

            try
            {

                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, "44", 1, 2);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, "00", 3, 4);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, arquivo.FK_Emissor.CodigoEmissor, 5, 9);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, string.Empty.PadLeft(5, ' '), 10, 13);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, dataRemessa, 14, 21);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, numRemessa.PadLeft(6, '0'), 22, 27);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, LAB5Utils.DataUtils.RetornaDataYYYYMMDD(DateTime.Now), 28, 35);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, situacao, 36, 36);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, motivo, 37, 38);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, "986", 39, 41);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, string.Empty, 42, 42);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, totalRegistros.ToString().PadLeft(15, '0'), 43, 57);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, totalTrnsacoesEmReal.ToString().PadLeft(15, '0'), 58, 72);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, valorTransacoesEmReal.ToString().Replace(',', ' ').Replace('.', ' ').ToString().PadLeft(15, '0'), 73, 87);
                //CORRIGIR AQUI retorno = LAB5Utils.ArquivoUtils.AlterarInformacao(retorno, new string('0',8), 88, 95);
                //CORRIGIR AQUI retorno = LAB5Utils.ArquivoUtils.AlterarInformacao(retorno, new string('0',8), 96, 110);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, "007", 111, 113);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, totalTransacoesEmDolar.ToString().PadLeft(8, '0'), 114, 121);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, valorTransacoesEmDolar.ToString().Replace(',', ' ').Replace('.', ' ').ToString().PadLeft(15, '0'), 122, 136);
                //CORRIGIR AQUI retorno = LAB5Utils.ArquivoUtils.AlterarInformacao(retorno, new string('0',8), 137, 144);
                //CORRIGIR AQUI retorno = LAB5Utils.ArquivoUtils.AlterarInformacao(retorno, new string('0',8), 145, 159);
                valorDetail = LAB5Utils.ArquivoUtils.AlterarInformacao(valorDetail, string.Empty.PadLeft(9, ' '), 160, 168);

                int idRegistroDetail = new RegistroDAO().RegistroPorArquivo(arquivo.IdArquivo).Where(r => r.ChaveRegistro.Equals("REGISTRO_E44_0")).First().IdRegistro;
                InformacaoRegistro detail = new InformacaoRegistro(idRegistroDetail, arquivo.IdArquivo, "0", LAB5Utils.StringUtils.Zip(valorDetail), false);

                infRegDAO.Salvar(detail);

                return detail;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar detail para o arquivo retorno." + ex.Message +" - " + ex.InnerException.Message);
            }
        }

        private InformacaoRegistro MontarTrailer()
        {
            TransacoesEloDAO transacaoDAO = new TransacoesEloDAO(arquivo.IdEmissor);
            InformacaoRegistro trailerIncoming = infRegDAO.BuscarTrailerArquivo(arquivo.IdArquivo);
            if (trailerIncoming.Equals(null)) throw new Exception("Trailer não localizado");
            trailerIncoming.IdInformacaoRegistro = 0;
            int totalTransacoesCredReal, totalTransacoesCredDolar, totalTransacoesDebReal, totalTransacoesDebDolar;
            decimal valorTransacaoesCredReal, valorTransacoesCredDolar, valorTransacoesDebReal, valorTransacoesDedDolar;

            string trailerReal = LAB5Utils.StringUtils.Unzip(trailerIncoming.Valor);

            totalTransacoesCredReal = transacaoDAO.TransacoesCredito(arquivo.NomeArquivo).Where(t=>t.CodigoMoeda == 986).ToList().Count;
            valorTransacaoesCredReal = transacaoDAO.TransacoesCredito(arquivo.NomeArquivo).Where(t => t.CodigoMoeda == 986).ToList().Sum(t => t.Valor);
            totalTransacoesCredDolar = transacaoDAO.TransacoesCredito(arquivo.NomeArquivo).Where(t => t.CodigoMoeda == 840).ToList().Count;
            valorTransacoesCredDolar = transacaoDAO.TransacoesCredito(arquivo.NomeArquivo).Where(t => t.CodigoMoeda == 840).ToList().Sum(t => t.Valor);
            totalTransacoesDebReal = transacaoDAO.TransacoesDebito(arquivo.NomeArquivo).Where(t => t.CodigoMoeda == 986).ToList().Count;
            totalTransacoesDebDolar = transacaoDAO.TransacoesDebito(arquivo.NomeArquivo).Where(t => t.CodigoMoeda == 840).ToList().Count;
            valorTransacoesDebReal = transacaoDAO.TransacoesDebito(arquivo.NomeArquivo).Where(t => t.CodigoMoeda == 986).Sum(t => t.Valor);
            valorTransacoesDedDolar = transacaoDAO.TransacoesDebito(arquivo.NomeArquivo).Where(t => t.CodigoMoeda == 840).Sum(t => t.Valor);

            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, totalTransacoesCredReal.ToString().PadLeft(8, '0'), 9, 16);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, valorTransacaoesCredReal.ToString().Replace('.', ' ').Replace(',', ' ').PadLeft(15, '0'), 17, 31);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, totalTransacoesDebReal.ToString().PadLeft(8, '0'), 32, 39);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, valorTransacoesDebReal.ToString().Replace('.', ' ').Replace(',', ' ').PadLeft(15, '0'), 40, 54);

            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, totalTransacoesCredDolar.ToString().PadLeft(8, '0'), 55, 62);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, valorTransacoesCredDolar.ToString().Replace('.', ' ').Replace(',', ' ').PadLeft(15, '0'), 63, 77);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, totalTransacoesDebDolar.ToString().PadLeft(8, '0'), 78, 85);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, valorTransacoesDedDolar.ToString().Replace('.', ' ').Replace(',', ' ').PadLeft(15, '0'), 86, 100);

            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal,totalRegistros.ToString().PadLeft(8,'0'),101,108);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, new string(' ', 8), 109, 116);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, new string(' ', 15), 117, 131);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal, new string(' ', 36), 132, 167);
            trailerReal = LAB5Utils.ArquivoUtils.AlterarInformacao(trailerReal,"2", 168, 168);

            trailerIncoming.Valor = LAB5Utils.StringUtils.Zip(trailerReal);

            try
            {
                infRegDAO.Salvar(trailerIncoming);
                return trailerIncoming;
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível gerar o trailer do arquivo de retorno. " + ex.Message + " - " + ex.InnerException);
            }

        }

        public bool MontarArquivoRetorno(string diretorioDestino, string situacaoRemessa, string motivo)
        {
            StreamWriter sw;

            if (Directory.Exists(diretorioDestino))
            {
                sw = new StreamWriter(diretorioDestino);
            }
            else
                throw new Exception("O Diretório de destino do arquivo não existe");

            if (!situacaoRemessa.ToUpper().Equals("A") || !situacaoRemessa.ToUpper().Equals("R"))
                throw new Exception("Valor inválido para situação da remessa. Valores permitidos: A e R");

            InformacaoRegistro header, detail, trailer;
            try
            {
                header  = MontarHeader();
                detail  = MontarDetail(dataRemessa,numRemessa,situacaoRemessa,motivo);
                trailer = MontarTrailer();
                Validar(header,detail,trailer);

                sw.WriteLine(LAB5Utils.StringUtils.Unzip(header.Valor));
                sw.WriteLine(LAB5Utils.StringUtils.Unzip(detail.Valor));
                sw.WriteLine(LAB5Utils.StringUtils.Unzip(trailer.Valor));
                sw.Flush();
                sw.Close();

                return true;

            }catch(Exception ex)
            {
                throw new Exception("Erro ao montar o arquivo. " + ex.Message);
            }
            
        }

        private void Validar(InformacaoRegistro header, InformacaoRegistro detail, InformacaoRegistro trailer)
        {
            if (header != null && detail != null && trailer != null)
            {
                string headerReal, detailReal, trailerReal;
                headerReal = LAB5Utils.StringUtils.Unzip(header.Valor);
                detailReal = LAB5Utils.StringUtils.Unzip(detail.Valor);
                trailerReal = LAB5Utils.StringUtils.Unzip(trailer.Valor);

                if (LAB5Utils.ArquivoUtils.ExtrairInformacao(headerReal, 5, 12) != LAB5Utils.ArquivoUtils.ExtrairInformacao(detailReal, 14, 21))
                    throw new Exception("Data de remessa difere no header e no trailer");
                if (int.Parse(LAB5Utils.ArquivoUtils.ExtrairInformacao(headerReal, 13, 16)) != int.Parse(LAB5Utils.ArquivoUtils.ExtrairInformacao(detailReal, 22, 27)))
                    throw new Exception("Número de remessa difere no header e no trailer");

            }
            else
                throw new Exception("Estrutura do arquivo incorreta.");
        }


    }
}
