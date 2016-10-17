using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class LayoutDAO
    {
        private class DAO : AbstractCrudDao<Layout> { }

        DAO _dao = new DAO();


        public void Salvar(Layout layout)
        {
            try
            {
                if (layout.IdLayout == 0)
                {
                    _dao.Add(layout);
                    _dao.CommitChanges();
                }
                else
                {
                    _dao.Update(layout, layout.IdLayout);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<Layout> ListarTodos()
        {
            return _dao.GetAll();
        }

        public Layout Buscar(string nome)
        {
            return _dao.Find(x => x.NomeLayout.Equals(nome)).FirstOrDefault();
        }

        public Layout Buscar(int id)
        {
            return _dao.Get(id);
        }
    }
}
