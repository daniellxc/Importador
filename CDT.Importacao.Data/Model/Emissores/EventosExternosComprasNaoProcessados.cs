using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model.Emissores
{
    [Table("EventosExternosComprasNaoProcessados")]
    public class EventosExternosComprasNaoProcessados
    {
        [Key]
        [Column("Id_EventoCompra")]
        public int IdEventoCompra { get; set; }

        [Column("Id_Autorizacao")]
        public int IdAutorizacao { get; set; }

        [Column("Cartao")]
        public string Cartao { get; set; }

        [Column("Nome_Estabelecimento_VISA")]
        public string NomeEstabelecimento { get; set; }

        [Column("CodigoAutorizacao")]
        public string CodigoAutorizacao { get; set; }

        [Column("MCC")]
        public Int16 MCC { get; set; }

        [Column("DataCompra")]
        public DateTime DataCompra { get; set; }

        [Column("ValorCompra")]
        public decimal ValorCompra { get; set; }
    }
}
