using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model.Emissores
{
    [Table("Cartoes")]
    public class Cartoes
    {
        [Column("Id_Cartao")]
        [Key]
        public  int IdCartao { get; set; }

        [Column("Cartao")]
        public string Cartao { get; set; }

        
        [Column("CartaoHash", TypeName = "bigint")]
        public long CartaoHash { get; set; }

       
    }
}
