using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class TipoRegistroDAO
    {
        private class DAO : AbstractCrudDao<TipoRegistro> { }

        DAO _dao = new DAO();

        public List<TipoRegistro> ListarTodos()
        {
            return _dao.GetAll();
        }
    }
}
