using CDT.Importacao.Data.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model
{
    [Table("TipoSubcampo")]
    public class TipoSubcampo
    {
        [Column("idTipoSubcampo")]
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTipoSubcampo { get; set; }

        [Column("chaveTipoSubcampo")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public string ChaveTipoSubcampo { get; set; }

        [Column("nomeTipoSubcampo")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public string NomeTipoSubcampo { get; set; }

    }
}
