using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model
{
    [Table("Registro")]
    public class Registro
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idRegistro")]
        public int IdRegistro { get; set; }

        [Column("idLayout")]
        public int IdLayout { get; set; }

        [Column("idTipoRegistro")]
        public int IdTipoRegistro { get; set; }

        [Column("chaveRegistro")]
        public string ChaveRegistro { get; set; }

        [Column("nomeRegistro")]
        public string NomeRegistro { get; set; }

        [ForeignKey("IdLayout")]
        public virtual Layout FK_Layout { get; set; }

        [ForeignKey("IdTipoRegistro")]
        public virtual TipoRegistro FK_TipoRegistro { get; set; }

        public virtual ICollection<Campo> Campos { get; set; }
    }
}
