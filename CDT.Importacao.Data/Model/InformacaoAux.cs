using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Model
{
    public class InformacaoAux
    {
       
        public int IdInformacaoRegistro { get; set; }

       
        public int IdRegistro { get; set; }

       
        public int IdArquivo { get; set; }

      
        public string Chave { get; set; }

       
        public byte[] Valor { get; set; }
    }
}
