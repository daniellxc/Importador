using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model
{ 
    [Table("Log")]
    public  class Log
    {
    
    [Column("idLog")]
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdLog { get; set; }

    [Column("date")]
    public DateTime Date { get; set; }

    [Column("source")]
    public string Source { get; set; }

    [Column("level")]
    public string Level { get; set; }

        [Column("user")]
        public string User{ get; set; }

        [Column("message")]
    public string Message{ get; set; }
}
}
