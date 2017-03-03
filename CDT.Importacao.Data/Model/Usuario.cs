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
    [Table("Usuario")]
    public class Usuario
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("idUsuario")]
        public int IdUsuario { get; set; }

        [Column("nome")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public string Nome { get; set; }

        [Column("login")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public string Login { get; set; }

        [Column("senha")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public string Senha { get; set; }


        [Column("ativo")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public bool Ativo { get; set; }

        [Column("admin")]
        [Required(ErrorMessage = Constantes.MSG_CAMPO_OBRIGATORIO)]
        public bool Admin { get; set; }
    }
}
