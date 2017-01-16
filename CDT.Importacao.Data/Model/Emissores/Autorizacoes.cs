using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model.Emissores
{
    [Table("Autorizacoes")]
    public class Autorizacoes
    {
        [Column("Id_Autorizacao")]
        [Key]
        public int IdAutorizacao { get; set; }

        [Column("CodigoAutorizacao")]
        public string CodigoAutorizacao { get; set; }

        [Column("DataAutorizacao")]
        public DateTime DataAutorizacao { get; set; }

        [Column("NSUOrigem")]
        public int NSUOrigem { get; set; }

        [Column("NumeroEstabelecimento")]
        public decimal NumeroEstabelecimento { get; set; }

        [Column("Cartao")]
        public string  Cartao{ get; set; }

        [Column("ReferenceNumber")]
        public string ReferenceNumber { get; set; }

        [Column("Valor")]
        public decimal Valor { get; set; }
    }
}
