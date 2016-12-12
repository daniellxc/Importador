using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using CDT.Importacao.Data.Utils.Quartz;
using CDT.Importacao.Data.Utils.Quartz.Jobs;
using CDT.Importacao.Data.Utils.Quartz.Schedulers;
using LAB5;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Business
{
    public class AgendamentoBO
    {

        public void Salvar(Agendamento agendamento)
        {
            try
            {
                new AgendamentoDAO().Salvar(agendamento);
                IniciarAgendamento(agendamento);

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        public  void IniciarAgendamentosAtivos()
        {
            foreach(Agendamento agd in new AgendamentoDAO().ListarAtivos())
            {
                IniciarAgendamento(agd);
            }
        }

        public bool IniciarAgendamento(Agendamento agendamento)
        {
            try
            {
               
                CDTJob job = (CDTJob)LAB5Utils.ReflectionUtils.TypeActivator("CDT.Importacao.Data", agendamento.JobClass);
                job.Start(agendamento.IdAgendamento,agendamento.CronExpression);

                return true;
            }
            catch
            {
                throw;
            }
        }

        public void PararAgendamento(string job, string groupJob)
        {
            try
            {
                CDTScheduler.DeleteJob(job, groupJob);
            }
            catch
            {
                throw;
            }

        }

        public bool StatusAgendamento(string job, string groupJob)
        {
            return CDTScheduler.JobExists(job, groupJob);
        }

        public DateTime ProximaExecucao(string job, string groupJob)
        {
            return CDTScheduler.NextExecutionTime(job, groupJob);
        }
    }
}
