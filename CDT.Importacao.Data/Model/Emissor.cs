using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model
{
    [Table("Emissor")]
    public class Emissor
    {
        [Key]
        [Column("idEmissor")]
        public int IdEmissor { get; set; }

        [Column("nomeEmissor")]
        public string NomeEmissor { get; set; }

        [Column("ipBaseEmissor")]
        public string IpBaseEmissor { get; set; }

        [Column("nomeBaseEmissor")]
        public string NomeBaseEmissor { get; set; }

        [Column("codigoEmissor")]
        public string CodigoEmissor { get; set; }


    }
}
