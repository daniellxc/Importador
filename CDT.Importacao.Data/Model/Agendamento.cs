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
    [Table("Agendamento")]
    public  class Agendamento
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idAgendamento")]
        public int IdAgendamento { get; set; }

        [Column("jobClass")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public string JobClass { get; set; }

        [Column("cronExpression")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public string CronExpression { get; set; }

        [Column("dataCriacao")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public DateTime DataCriacao{ get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; }
    }
}
