using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model
{
    [Table("Informacao")]
    public class Informacao
    {
        [Column("idInformacao")]
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idInformacao { get; set; }

        [Column("idArquivo")]
        public int idArquivo { get; set; }

        [Column("idCampo")]
        public int idCampo { get; set; }

        [Column("valor")]
        public string valor { get; set; }
       
        //[ForeignKey("IdArquivo")]
        //public virtual Arquivo FK_Arquivo { get; set; }

        //[ForeignKey("IdCampo")]
        //public virtual Campo FK_Campo { get; set; }

        public Informacao()
        {

        }
    }
}
