using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class SubcampoDAO
    {
        private class DAO : AbstractCrudDao<Subcampo>
        {
            public DAO(Contexto ctx) : base(ctx)
            {

            }
        }

        DAO _dao = new DAO(new Contexto());


        public void Salvar(Subcampo subcampo)
        {
            try
            {
                if (subcampo.IdSubcampo == 0)
                {
                    _dao.Add(subcampo);
                    _dao.CommitChanges();
                }
                else
                {
                    _dao.Update(subcampo, subcampo.IdSubcampo);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void Excluir(int idSubcampo)
        {
            try
            {
                _dao.Delete(_dao.Get(idSubcampo));
            }
            catch (DbUpdateException dbex)
            {
                throw new Exception("Erro ao excluir." + dbex.Message);
            }
            catch
            {
                throw;
            }
        }

        public List<Subcampo> ListarTodos()
        {
            return _dao.GetAll();
        }

        public Subcampo Buscar(string nome)
        {
            return _dao.Find(x => x.NomeSubcampo.Equals(nome)).FirstOrDefault();
        }

        public Subcampo Buscar(int id)
        {
            return _dao.Get(id);
        }
    }
}
