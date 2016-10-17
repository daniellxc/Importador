using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public  class InformacaoRegistroDAO
    {
        private class DAO : AbstractCrudDao<InformacaoRegistro> { }

        DAO _dao = new DAO();


       

        public void Salvar(InformacaoRegistro informacaoRegistro)
        {
            try
            {
                if (informacaoRegistro.IdInformacaoRegistro == 0)
                {
                    _dao.Add(informacaoRegistro);
                    _dao.CommitChanges();
                }
                else
                {
                    _dao.Update(informacaoRegistro, informacaoRegistro.IdInformacaoRegistro);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Salvar(List<InformacaoRegistro> InformacaoRegistro)
        {
            
            try
            {
                _dao.BulkInsert(InformacaoRegistro);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        public List<InformacaoRegistro> ListarTodos()
        {
            return _dao.GetAll();
        }


        public InformacaoRegistro Buscar(int id)
        {
            return _dao.Get(id);
        }
    }
}
