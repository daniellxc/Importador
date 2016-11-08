using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class TipoDadoDAO
    {
        private class DAO : AbstractCrudDao<TipoDado>
        {
            public DAO(Contexto ctx) : base(ctx)
            {

            }
        }

        DAO _dao = new DAO(new Contexto());


        public void Salvar(TipoDado tipoDado)
        {
            try
            {
                if (tipoDado.IdTipoDado == 0)
                {
                    _dao.Add(tipoDado);
                    _dao.CommitChanges();
                }
                else
                {
                    _dao.Update(tipoDado, tipoDado.IdTipoDado);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<TipoDado> ListarTodos()
        {
            return _dao.GetAll();
        }

        public TipoDado Buscar(string nome)
        {
            return _dao.Find(x => x.NomeTipoDado.Equals(nome)).FirstOrDefault();
        }

        public TipoDado Buscar(int id)
        {
            return _dao.Get(id);
        }
    }
}
