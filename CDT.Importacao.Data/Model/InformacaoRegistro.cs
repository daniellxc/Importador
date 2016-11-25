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
        public byte[] Valor { get; set; }

        [Column("flagErro")]
        public bool FlagErro { get; set; }

        public InformacaoRegistro()
        {

        }

        public InformacaoRegistro(int idRegistro,int idArquivo , string chave, byte[] valor)
        {
            this.IdArquivo = idArquivo;
            this.IdRegistro = idRegistro;
            this.Chave = chave;
            this.Valor = valor;
         
        }

        public InformacaoRegistro(int idRegistro, int idArquivo, string chave, byte[] valor, bool flagErro)
        {
            this.IdArquivo = idArquivo;
            this.IdRegistro = idRegistro;
            this.Chave = chave;
            this.Valor = valor;
            this.FlagErro = flagErro;
        }
    }
}
