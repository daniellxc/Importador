using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model
{
    [Table("StatusArquivo")]
    public class StatusArquivo
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idStatusArquivo")]
        public  int IdStatusArquivo { get; set; }

        [Column("nomeStatus")]
        public string NomeStatus{ get; set; }
    }
}
