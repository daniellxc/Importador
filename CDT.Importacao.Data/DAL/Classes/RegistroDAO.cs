using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class RegistroDAO
    {
        private class DAO : AbstractCrudDao<Registro> { }

        DAO _dao = new DAO();


        public void Salvar(Registro registro)
        {
            try
            {
                if (registro.IdRegistro == 0)
                {
                    _dao.Add(registro);
                    _dao.CommitChanges();
                }
                else
                {
                    _dao.Update(registro, registro.IdRegistro);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<Registro> ListarTodos()
        {
            return _dao.GetAll();
        }

        public Registro Buscar(string chave)
        {
            return _dao.Find(x => x.ChaveRegistro.Equals(chave)).FirstOrDefault();
        }

        public Registro Buscar(int id)
        {
            return _dao.Get(id);
        }
    }
}
