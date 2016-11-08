using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Utils
{
    public static class Constantes
    {
        public const string MSG_CAMPO_OBRIGATORIO = "Campo obrigatório";

        #region COD_TRANSACOES
        public const string TE01 = "01";
        public const string TE05 = "05";
        public const string TE06 = "06";
        public const string TE10 = "10";
        public const string TE15 = "15";
        public const string TE16 = "16";
        public const string TE20 = "20";
        public const string TE25 = "25";
        public const string TE26 = "26";
        public const string TE35 = "35";
        public const string TE36 = "36";
        public const string TE40 = "40";
        public const string TE44 = "44";
        #endregion

        public class LiquidacaoInternacionalElo
        {
            public const string HEADER_GRUPO_CARTOES = "01";
            public const string DETALHE_SEM_SDR = "05";
            public const string DETALHE_COM_SDR = "06";
            public const string SDR = "07";
            public const string CONVERSAO_MOEDAS = "08";
            public const string TRAILER_GRUPO_CARTOES = "09";
            public const string HEADER_TAXAS = "20";
            public const string DETALHE_TAXAS = "25";
            public const string TRAILER_TAXAS = "29";
            public const string HEADER_CORRECOES = "30";
            public const string DETALHE_CORRECOES = "35";
            public const string TRAILER_CORRECOES = "39";
            public const string HEADER_INTERCAMBIO = "40";
            public const string DETALHE_INTERCAMBIO = "45";
            public const string TRAILER_INTERCAMBIO = "49";
            public const string HEADER_DESEMBOLSO = "50";
            public const string DETALHE_DESEMBOLSO = "52";
            public const string TRAILER_DESEMBOLSO = "55";
            public const string TRAILER_ARQUIVO = "99";
        }

        public const Int16 BUFFER_LIMIT = 1000;

        public const string SPLITTER_REGISTRO = "{*}";
        public const string SPLITTER_LINHA = "||";
    }
}
