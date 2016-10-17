using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class CampoDAO
    {
        private class DAO : AbstractCrudDao<Campo> { }

        DAO _dao = new DAO();


        public void Salvar(Campo campo)
        {
            try
            {
                if (campo.IdCampo == 0)
                {
                    _dao.Add(campo);
                    _dao.CommitChanges();
                }
                else
                {
                    _dao.Update(campo, campo.IdCampo);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<Campo> ListarTodos()
        {
            return _dao.GetAll();
        }

        public Campo Buscar(string nome)
        {
            return _dao.Find(x => x.NomeCampo.Equals(nome)).FirstOrDefault();
        }

        public Campo Buscar(int id)
        {
            return _dao.Get(id);
        }
    }
}
