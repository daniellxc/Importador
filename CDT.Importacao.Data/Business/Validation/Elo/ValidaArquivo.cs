using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using CDT.Importacao.Data.Model.Emissores;
using LAB5;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Business.Validation.Elo
{
    /// <summary>
    /// Classe contendo as validações pela Elo na conciliação de transações nacionais. 
    /// </summary>
    public class ValidaArquivo
    {

        
        TransacaoElo transacao = null;
        List<ErroValidacaoArquivo> erroList = new List<ErroValidacaoArquivo>();
        AutorizacaoEvtExternoCompraNaoProcessado autEvtExt = null;
        string tipoTransacao = "";

        public ValidaArquivo(ref TransacaoElo transacao)
        { 
            this.transacao = transacao;
             
        }

       
        /// <summary>
        /// Batimento do número do cartão enviado na conciliação com o registrado na autorização.
        /// </summary>
        /// <param name="numeroCartao"></param>
        /// <param name="codigoAutorizacao"></param>
        /// <param name="codigoMCC"></param>
        /// <returns></returns>
        public  string ValidacaoE1(string numeroCartao)
        {
            string numCartao = "";
            if (numeroCartao.Length > 19 || numeroCartao.Length < 16)
                return "";
            else
                if (numeroCartao.Length == 19)
                numCartao = numeroCartao.Substring(16, 3) == "000" ? numeroCartao.Substring(0, 16) : numeroCartao;

            if (autEvtExt == null)
                return "";
            return autEvtExt.Cartao;
        }


        public  bool ValidacaoE2()
        {
            //verificar posteriormente
            return true;
        }

        /// <summary>
        /// Verifica número de referência da transação
        /// </summary>
        /// <returns></returns>
        public bool ValidacaoE3()
        {
            string bin = autEvtExt.Cartao.Substring(0, 6);

            if (autEvtExt.ReferenceNumber.Length != 23 || autEvtExt.ReferenceNumber.Equals(new string('0', 23)))
                return false;

            if (autEvtExt.ReferenceNumber.Substring(0, 1) != "2" && autEvtExt.ReferenceNumber.Substring(0, 1) != "7")
                return false;

            if (autEvtExt.ReferenceNumber.Substring(1, 6) != bin)
                return false;

          
            string ano = autEvtExt.DataAutorizacao.Year.ToString().Substring(3, 1);
            string dia = autEvtExt.DataAutorizacao.DayOfYear.ToString().PadLeft(3,'0');

            if (autEvtExt.ReferenceNumber.Substring(7, 4) != ano + dia)
                return false;
            //if (autorizacao.ReferenceNumber.Substring(22, 1) != digitoVerificador)

            return true;
           

        }

        /// <summary>
        /// Verifica se existe autorização com a data informada na conciliação.
        /// </summary>
        /// <returns></returns>
        public bool ValidacaoE4()
        {
            return autEvtExt != null;

        }

        /// <summary>
        /// Verifica se o valor da venda é compatível com o valor informado na conciliação.
        /// </summary>
        /// <param name="valorVenda"></param>
        /// <returns></returns>
        public bool ValidacaoE5(decimal valorVenda)
        {
            return autEvtExt.Valor.Equals(valorVenda);
                
        }


        /// <summary>
        /// Verifica se o tipo de transação informado é válido.
        /// </summary>
        /// <param name="tipoTransacaoNaConciliaca"></param>
        /// <returns></returns>
        public bool ValidacaoE6(string tipoTransacaoNaConciliacao)
        {
            return
                  tipoTransacaoNaConciliacao == "1" ||
                  tipoTransacaoNaConciliacao == "2" ||
                  tipoTransacaoNaConciliacao == "3" ||
                  tipoTransacaoNaConciliacao == "4" ||
                  tipoTransacaoNaConciliacao == "5" ||
                  tipoTransacaoNaConciliacao == "6";
            
        }


        public bool ValidacaoE7(int codigoMoeda)
        {
            return codigoMoeda == 986 || codigoMoeda == 840;
               
        }

        public bool ValidacaoE8(string codigoCredenciadora)
        {
            return codigoCredenciadora != null && LAB5Utils.StringUtils.IsNumber(codigoCredenciadora);
               
        }

        public bool ValidacaoE9()
        {
            if (tipoTransacao == "1")
                return autEvtExt != null;
            return true;
                   

        }

        public bool Validacao10(string codigoProduto)
        {
            //desacoplar esta "lista" depois.
            return
                codigoProduto == "009" ||
                codigoProduto == "013" ||
                codigoProduto == "014" ||
                codigoProduto == "015" ||
                codigoProduto == "016" ||
                codigoProduto == "017" ||
                codigoProduto == "018" ||
                codigoProduto == "045" ||
                codigoProduto == "046" ||
                codigoProduto == "059" ||
                codigoProduto == "058" ||
                codigoProduto == "065" ||
                codigoProduto == "066" ||
                codigoProduto == "070" ||
                codigoProduto == "071" ||
                codigoProduto == "072" ||
                codigoProduto == "086" ||
                codigoProduto == "087" ||
                codigoProduto == "088" ||
                codigoProduto == "089" ||
                codigoProduto == "091" ||
                codigoProduto == "330" ||
                codigoProduto == "377";
             
        }

        public bool Validacao11(string numeroEstabelecimento)
        {
            return autEvtExt.NumeroEstabelecimento.Equals(numeroEstabelecimento);
                
        }


        public bool Validacao12(string nomeEstabelecimento)
        {
            if (autEvtExt != null)
                return autEvtExt.NomeEstabelecimento.Equals(nomeEstabelecimento);
            //throw new Exception("E12 - Nome do EC divergente entre autorizacao e liquidacao. Ref.Number: " + autorizacao.ReferenceNumber);
            return false;
        }

        public bool Validacao13(string mcc)
        {
            if(autEvtExt != null)
                return autEvtExt.MCC.Equals(mcc);
            // throw new Exception("E13 - Nome do EC divergente entre autorizacao e liquidacao. Ref.Number: " + autorizacao.ReferenceNumber);
            return false;
          
        }

        public bool Validacao14(string nsuOrigem)
        {
            if (autEvtExt != null)
                if (tipoTransacao == "1")
                    return autEvtExt.NSUOrigem.Equals(nsuOrigem);
                else return true;
            return false;
                        //throw new Exception("E14 - Numero logico do equipamento divergente entre autorizacao e liquidacao. Ref.Number: " + autorizacao.ReferenceNumber);

        }

        public bool Validacao15(string qtdDiasLiquidacao, string TE)
        {
            int valorDias = 0;
            if (qtdDiasLiquidacao != null && LAB5Utils.StringUtils.IsNumber(qtdDiasLiquidacao))
            {
                valorDias = int.Parse(qtdDiasLiquidacao);

                if (TE == "05" || TE == "06")
                    return valorDias > 0;
                else
                    return true;
                        //throw new Exception("E15 - Valor invalido para quantidade de dias para liquidacao. Ref.Number: " + autorizacao.ReferenceNumber);
            }
            return false;
            //throw new Exception("E15 - Quantidade de dias para liquidacao em formato invalido. Ref.Number: " + autorizacao.ReferenceNumber);
        }

        public bool Validacao16(string valorIntercambio)
        {
            if (LAB5Utils.StringUtils.IsNumber(valorIntercambio))
                return decimal.Parse(valorIntercambio) <= 0;
            return false;       //throw new Exception("E16 - Valor de intercambio invalido. Ref.Number: " + autorizacao.ReferenceNumber);
            //throw new Exception("E16 - Valor de intercambio em formato invalido. Ref.Number: " + autorizacao.ReferenceNumber);
        }

        public bool Validacao17(string dataMovimento)
        {
            if (LAB5Utils.StringUtils.IsNumber(dataMovimento))
            {
                try
                {
                    LAB5Utils.DataUtils.RetornaData(dataMovimento);
                    return true;

                }
                catch
                {
                    return false;// throw new Exception("E17 - Data de movimento em formato invalido. Ref.Number: " + autorizacao.ReferenceNumber);
                }
            }

            return false;

        }
        
        
        public List<ErroValidacaoArquivo> Validar(AutorizacaoEvtExternoCompraNaoProcessado autEvtExt, long idInformacaoRegistro)
        {
            this.autEvtExt = autEvtExt;
                        
             if (autEvtExt != null)
            {
                if ((transacao.Cartao = this.ValidacaoE1(transacao.Cartao)) == "")
                    AddErroList(idInformacaoRegistro,"E01 - Numero do cartao invalido. Ref.Number:" + autEvtExt.ReferenceNumber);
                //validacao E2
                if (!this.ValidacaoE2())
                    AddErroList(idInformacaoRegistro, "E02 - Digito do cartao invalido. Ref.Number:" + autEvtExt.ReferenceNumber);
                ////validacao E3
                if (!this.ValidacaoE3())
                    AddErroList(idInformacaoRegistro, "E03 - Numero de referencia da transacao inconsistente. Ref.Number:" + autEvtExt.ReferenceNumber);
                ////validacao E4
                if (!this.ValidacaoE4())
                    AddErroList(idInformacaoRegistro, "E04 - Data da venda inconsistente no Registro 0. Ref.Number:" + autEvtExt.ReferenceNumber);
                ////validacao E5
                if (!this.ValidacaoE5(transacao.Valor))
                    AddErroList(idInformacaoRegistro, "E05 - Valor da venda inconsistente. Ref.Number:" + autEvtExt.ReferenceNumber);
                ////validacao E6
                if (!this.ValidacaoE6(transacao.CicloApresentacao.ToString()))
                    AddErroList(idInformacaoRegistro, "E06 - Tipo de transacao invalido. Ref.Number:" + autEvtExt.ReferenceNumber);
                ////validacao E7
                if (!this.ValidacaoE7(transacao.CodigoMoeda))
                    AddErroList(idInformacaoRegistro, "E07 - Codigo de moeda diferente de 986 e 840. Ref.Number:" + autEvtExt.ReferenceNumber);
                ////validacao E8
                if (!this.ValidacaoE8(transacao.CodigoCredenciadora))
                    AddErroList(idInformacaoRegistro, "E08 - Codigo da credenciadora inexistente ou inconsistente. Ref.Number:" + autEvtExt.ReferenceNumber);
                ////validacao E9
                if (!this.ValidacaoE9())
                    AddErroList(idInformacaoRegistro, "E09 - Codigo de autorizacao inexistente ou inconsistente. Ref.Number:" + autEvtExt.ReferenceNumber);
                ////validacaoE10
                if (!this.Validacao10(transacao.IdProduto.ToString()))
                    AddErroList(idInformacaoRegistro, "E10 - Codigo de produto inconsistente. Ref.Number:" + autEvtExt.ReferenceNumber);
                ////validacaoE11
                if (!this.Validacao11(transacao.NumeroEstabelecimento))
                    AddErroList(idInformacaoRegistro, "E11 - Numero do estabelecimento inconsistente. Ref.Number:" + autEvtExt.ReferenceNumber);
                ////validacaoE12
                if (!this.Validacao12(transacao.NomeEstabelecimento))
                    AddErroList(idInformacaoRegistro, "E12 - Nome do estabelecimento deve ser diferente de brancos. Ref.Number:" + autEvtExt.ReferenceNumber);
                ////validacaoE13
                if (!this.Validacao13(transacao.CodigoMCC.ToString()))
                    AddErroList(idInformacaoRegistro, "E13 - Codigo do ramo de atividade invalido ou inconsistente. Ref.Number:" + autEvtExt.ReferenceNumber);
                ////validacaoE14
                if (!this.Validacao14(transacao.NSUOrigem))
                    AddErroList(idInformacaoRegistro, "E14 - Numero logico do equipamento inconsistente. Ref.Number:" + autEvtExt.ReferenceNumber);
                ////validacaoE15
                if (!this.Validacao15(transacao.QtdDiasLiquidacao, transacao.TE))
                    AddErroList(idInformacaoRegistro, "E15 - Quantidade de dias para liquidacao financeira zerada ou inconsistente. Ref.Number:" + autEvtExt.ReferenceNumber);
                ////validacaoE16
                if (!this.Validacao16(LAB5Utils.StringUtils.OnlyNumbers(transacao.ValorIntercambio.ToString())))
                    AddErroList(idInformacaoRegistro, "E16 - Valor de intercambio inconsistente. Ref.Number:" + autEvtExt.ReferenceNumber);
                ////validacaoE17
                if (!this.Validacao17(LAB5Utils.DataUtils.RetornaDataYYYYMMDD(transacao.DataTransacao.Date)))
                    AddErroList(idInformacaoRegistro, "E17 - Data de movimento inconsistente no registro 0. Ref.Number:" + autEvtExt.ReferenceNumber);
            }
            else
                AddErroList(idInformacaoRegistro, "E01 - Erro crítico. Nao foi possivel localizar a autorizacao referente a transacao informada.");
           

            return erroList;
        }
        
        private void AddErroList(long idInformacaoRegistro,string erro)
        {
            erroList.Add(new ErroValidacaoArquivo(idInformacaoRegistro,erro));
        }
    }
}
