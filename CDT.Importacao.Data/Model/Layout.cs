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
    [Table("dbo.Layout")]
    public class Layout
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idLayout")]
        public int IdLayout { get; set; }

        [Column("nomeLayout")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO )]
        public string NomeLayout { get; set; }

        [Column("diretorioArquivo")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        [DataType(DataType.Upload)]
        public string DiretorioArquivo { get; set; }

        [Column("classeImportadora")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public string ClasseImportadora { get; set; }

        [Column("temRetorno")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public bool TemRetorno { get; set; }



        public virtual ICollection<Registro> Registros { get; set; }

    }
}
