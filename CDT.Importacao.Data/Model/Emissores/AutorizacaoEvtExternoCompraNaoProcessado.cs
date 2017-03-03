using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model.Emissores
{
    public class AutorizacaoEvtExternoCompraNaoProcessado
    {
        public int IdAutorizacao { get; set; }
        public string Cartao { get; set; }
        public string CartaoReal { get; set; }
        public string CodigoAutorizacao { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime DataAutorizacao { get; set; }
        public decimal Valor { get; set; }
        public decimal NumeroEstabelecimento { get; set; }
        public int NSUOrigem { get; set; }

        public string NomeEstabelecimento{ get; set; }
        public Int16 MCC { get; set; }
    }
}
