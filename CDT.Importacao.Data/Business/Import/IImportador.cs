using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Business.Import
{
    public interface IImportador
    {
        

         void Importar(Arquivo arquivo);
    }
}
