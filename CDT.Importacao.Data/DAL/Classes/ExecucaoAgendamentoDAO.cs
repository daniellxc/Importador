using CDT.Importacao.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.DAL.Classes
{
    public class ExecucaoAgendamentoDAO
    {
        private class DAO : AbstractCrudDao<ExecucaoAgendamento>
        {
            public DAO(Contexto ctx) : base(ctx)
            {

            }
        }

        DAO _dao = new DAO(new Contexto());


        public void Salvar(ExecucaoAgendamento exeAgd)
        {
            try
            {
                if (exeAgd.IdExecucaoAgendamento == 0)
                {
                    _dao.Add(exeAgd);
                    _dao.CommitChanges();
                }else
                {
                    _dao.Update(exeAgd, exeAgd.IdExecucaoAgendamento);
                }

            }catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<ExecucaoAgendamento> BuscarExecucoesDeUmAgendamento(int idAgendamento)
        {
            return _dao.Find(x => x.IdAgendamento == idAgendamento);
        }
    }
}
