using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
     public class TipoSubcampoDAO
    {
        private class DAO : AbstractCrudDao<TipoSubcampo>
        {
            public DAO(Contexto ctx) : base(ctx)
            {

            }
        }

        DAO _dao = new DAO(new Contexto());


        public void Salvar(TipoSubcampo TipoSubcampo)
        {
            try
            {
                if (TipoSubcampo.IdTipoSubcampo == 0)
                {
                    _dao.Add(TipoSubcampo);
                    _dao.CommitChanges();
                }
                else
                {
                    _dao.Update(TipoSubcampo, TipoSubcampo.IdTipoSubcampo);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void Excluir(int idTipoSubcampo)
        {
            try
            {
                _dao.Delete(_dao.Get(idTipoSubcampo));
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

        public List<TipoSubcampo> ListarTodos()
        {
            return _dao.GetAll();
        }

        public TipoSubcampo Buscar(string nome)
        {
            return _dao.Find(x => x.NomeTipoSubcampo.Equals(nome)).FirstOrDefault();
        }

        public TipoSubcampo Buscar(int id)
        {
            return _dao.Get(id);
        }
    }
}
