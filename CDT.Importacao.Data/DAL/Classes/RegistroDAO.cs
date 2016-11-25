using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class RegistroDAO
    {
        private class DAO : AbstractCrudDao<Registro>
        {
            public DAO(Contexto ctx) : base(ctx)
            {

            }
        }

        DAO _dao = new DAO(new Contexto());


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
                    _dao.Update(registro);
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

        public Registro Buscar(string chave, int idLayout)
        {
            return _dao.Find(x => x.ChaveRegistro.Equals(chave) && x.IdLayout == idLayout).FirstOrDefault();
        }

     

        public Registro Buscar(int id)
        {
            return _dao.Get(id);
        }

        public List<Registro> RegistroPorArquivo(int idArquivo)
        {
            var result = from a in _dao.GetContext().Set<Registro>()
                         join b in _dao.GetContext().Set<Layout>()
                         on a.IdLayout equals b.IdLayout
                         join c in _dao.GetContext().Set<Arquivo>()
                         on b.IdLayout equals c.IdLayout
                         where c.IdArquivo == idArquivo
                         select a;
            return result.ToList();
        }

        public List<Registro> RegistrosLayout(int idLayout)
        {
            return _dao.Find(x => x.IdLayout == idLayout);
        }

        public void Excluir(int IdRegistro)
        {
            try
            {
                _dao.Delete(_dao.Get(IdRegistro));
            }catch(DbUpdateException dbex)
            {
                throw new Exception("Erro ao excluir." + dbex.Message);
            }
            catch
            {
                throw;
            }
        }
    }
}
