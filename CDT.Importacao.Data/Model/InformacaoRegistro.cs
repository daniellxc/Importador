using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model
{
    [Table("InformacaoRegistro")]
    public class InformacaoRegistro
    {
        [Column("idInformacaoRegistro")]
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 IdInformacaoRegistro { get; set; }

        [Column("idRegistro")]
        public int IdRegistro { get; set; }

        [Column("idArquivo")]
        public int IdArquivo { get; set; }

        [Column("chave")]
        public string Chave { get; set; }

        [Column("valor")]
        public string Valor { get; set; }

        public InformacaoRegistro()
        {

        }

        public InformacaoRegistro(int idRegistro,int idArquivo , string chave, string valor)
        {
            this.IdArquivo = idArquivo;
            this.IdRegistro = idRegistro;
            this.Chave = chave;
            this.Valor = valor;
        }
    }
}
