using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using CDT.Importacao.Data.Utils.Quartz.Schedulers;
using CDT.Importacao.Data.Utils.Log;
using CDT.Importacao.Data.Business;
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Utils.Extensions;
using LAB5;

namespace CDT.Importacao.Data.Utils.Quartz.Jobs
{
    public class ReportFraudeEloJob : CDTJob
    {
        public object LAB5Util { get; private set; }

        public void Execute(IJobExecutionContext context)
        {
            string message = "";
            bool sucesso = true;
            int idAgendamento = 0;
            try
            {
                List<string> result;
                JobDataMap jobDataMap = context.JobDetail.JobDataMap;
                idAgendamento = jobDataMap.GetInt("idAgendamento");
                string dataParam = LAB5Utils.DataUtils.RetornaDataYYYYMMDD(DateTime.Now.AddDays(-1));
                result = new TransacoesEloDAO(85).GerarReportFraude(dataParam);

                message = "Liquidação Nacional ELO - Arquivo report de fraudes gerado com sucesso. " + (result.Count - 2).ToString().PadLeft(4, '0') + " trans. fraudulenta(s).";

                Logger.Info(this.ToString(), message, "QuartzJob");

            }
            catch(Exception ex)
            {
                message = "Erro ao executar importação automática do arquivo de Liquidação Nacional Elo. " + ex.GetAllMessages();
                sucesso = false;
                Logger.Warn(this.ToString(), message, "QuartzJob");
                throw ex;
            }
            finally
            {
                new ExecucaoAgendamentoBO().SalvarExecucaoAgendamento(idAgendamento, DateTime.Now, message, sucesso);
            }
        }

        public void Start(int idAgendamento, string cronExpression)
        {
            JobDataMap jobMap = new JobDataMap();
            jobMap.Add("idAgendamento", idAgendamento);
            if (cronExpression != string.Empty)
                CDTScheduler.StartJobSchedule<ReportFraudeEloJob>(jobMap, cronExpression, this.ToString() + idAgendamento.ToString(), "grp_" + this.ToString() + idAgendamento.ToString());
            else
                CDTScheduler.StartJobSchedule<ReportFraudeEloJob>(jobMap, this.ToString() + idAgendamento.ToString(), "grp_" + this.ToString() + idAgendamento.ToString());
        }
    }
}
