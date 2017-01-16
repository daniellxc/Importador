using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model
{
    [Table("ErroValidacaoArquivo")]
    public class ErroValidacaoArquivo
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idErroValidacaoArquivo")]
        public  int IdErroValidacaoArquivo { get; set; }

        [Column("idInformacaoRegistro")]
        public long IdInformacaoRegistro { get; set; }

        [Column("erro")]
        public string erro { get; set; }



        public ErroValidacaoArquivo()
        {

        }

        public ErroValidacaoArquivo(long idInformacaoRegistro, string erro)
        {
            this.IdInformacaoRegistro = idInformacaoRegistro;
            this.erro = erro;
        }
    }
}
