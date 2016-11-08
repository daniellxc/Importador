using CDT.Importacao.Data.Business.Import;
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testes
{
    [TestClass]
    public class ImportadorInternacionalEloTest
    {
        [TestMethod]
        public void TestarImportar()
        {
            Arquivo arquivo = new ArquivoDAO().Buscar(11);
            new ImportadorInternacionalElo().Importar(arquivo);
        }
    }
}
