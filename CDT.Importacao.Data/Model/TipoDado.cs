using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model
{
    [Table("TipoDado")]
    public class TipoDado
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idTipoDado")]
        public int IdTipoDado { get; set; }

        [Column("nomeTipoDado")]
        public string NomeTipoDado { get; set; }

        [Column("type")]
        public string Type { get; set; }
    }
}
