using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model
{
    [Table("TipoRegistro")]
    public class TipoRegistro
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idTipoRegistro")]
        public int IdTipoRegistro { get; set; }

        [Column("nomeTipoRegistro")]
        public string NomeTipoRegistro{ get; set; }
    }
}
