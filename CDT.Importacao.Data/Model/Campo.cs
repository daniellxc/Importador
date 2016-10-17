using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model
{
    [Table("Campo")]
    public class Campo
    {

        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idCampo")]
        public int IdCampo { get; set; }

        [Column("idRegistro")]
        public int IdRegistro { get; set; }

        [Column("idTipoDado")]
        public int IdTipoDado { get; set; }

        [Column("nomeCampo")]
        public string NomeCampo { get; set; }
        
        [Column("posInicio")]
        public int PosInicio { get; set; }

        [Column("posFim")]
        public int PosFim { get; set; }

        [ForeignKey("IdRegistro")]
        public virtual Registro FK_Registro { get; set; }

        [ForeignKey("IdTipoDado")]
        public virtual TipoDado FK_TipoDado { get; set; }



    }
}
