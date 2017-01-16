using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model
{
    [Table("ExecucaoAgendamento")]
    public class ExecucaoAgendamento
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idExecucaoAgendamento")]
        public int IdExecucaoAgendamento { get; set; }

        [Column("idAgendamento")]
        public int IdAgendamento { get; set; }

        [Column("resultado")]
        public string Resultado { get; set; }

        [Column("dataExecucao")]
        public DateTime DataExecucao { get; set; }

        [Column("sucesso")]
        public bool Sucesso { get; set; }

        [ForeignKey("IdAgendamento")]
        public virtual Agendamento FK_Agendamento { get; set; }
    }
}
