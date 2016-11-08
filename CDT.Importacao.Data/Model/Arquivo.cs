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
    [Table("Arquivo")]
    public class Arquivo
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idArquivo")]
        public int IdArquivo { get; set; }

        [Column("idLayout")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public int IdLayout { get; set; }

        [Column("idStatusArquivo")]
        public int IdStatusArquivo { get; set; }

        [Column("nomeArquivo")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        [DataType(DataType.Upload)]
        public string  NomeArquivo{ get; set; }

        [Column("dataImportacao")]
        [DataType(DataType.DateTime)]
        public DateTime DataImportacao { get; set; }

        [Column("dataRegistro")]
        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public DateTime DataRegistro { get; set; }

        [Column("idEmissor")]
        public int IdEmissor { get; set; }




        [ForeignKey("IdLayout")]
        public virtual Layout FK_Layout { get; set; }

        [ForeignKey("IdStatusArquivo")]
        public virtual StatusArquivo FK_StatusArquivo{ get; set; }

        [ForeignKey("IdEmissor")]
        public virtual Emissor FK_Emissor { get; set; }
    }
}
