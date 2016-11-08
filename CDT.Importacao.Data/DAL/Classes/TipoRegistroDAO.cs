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
        private class DAO : AbstractCrudDao<TipoRegistro>
        {
            public DAO(Contexto ctx) : base(ctx)
            {

            }
        }

        DAO _dao = new DAO(new Contexto());

        public List<TipoRegistro> ListarTodos()
        {
            return _dao.GetAll();
        }
    }
}
