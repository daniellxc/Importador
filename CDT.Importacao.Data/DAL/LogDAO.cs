using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL
{
    public class LogDAO
    {
        private class DAO : AbstractCrudDao<Log>
        {
            public DAO(Contexto ctx) : base(ctx)
            {

            }
        }

        DAO _dao = new DAO(new Contexto());

        public void Salvar(Log log)
        {
            try
            {
                _dao.Add(log);
                _dao.CommitChanges();

            }catch (Exception ex) { }
        }
    }
}
