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
    [Table("Subcampo")]
    public class Subcampo
    {
        [Column("idSubcampo")]
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdSubcampo { get; set; }

        [Column("idCampo")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public int IdCampo { get; set; }

        [Column("idTipoDado")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public int IdTipoDado { get; set; }

        [Column("idTipoSubcampo")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public int IdTipoSubcampo { get; set; }

        [Column("nomeSubcampo")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public string NomeSubcampo { get; set; }

        [Column("posInicio")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public int PosInicio { get; set; }

        [Column("posFim")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public int PosFim { get; set; }

        [ForeignKey("IdCampo")]
        public virtual Campo FK_Campo { get; set; }

        [ForeignKey("IdTipoDado")]
        public virtual TipoDado FK_TipoDado { get; set; }

        [ForeignKey("IdTipoSubcampo")]
        public virtual TipoSubcampo FK_TipoSubcampo { get; set; }
    }
}
