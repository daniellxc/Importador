using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Business;
using CDT.Importacao.Data.Model;
using LAB5;
using CDT.Importacao.Data.Utils.Log;
using CDT.Importacao.Data.Utils.Quartz.Schedulers;

namespace CDT.Importacao.Data.Utils.Quartz.Jobs
{
    public class LiquidacaoInternacionalEloJob : CDTJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ArquivoDAO arquivoDAO = new ArquivoDAO();
            ArquivoBO arquivoBO = null;
            Arquivo arquivo = null;
            int idAgendamento = 0;
            bool sucesso = false;
            string message = "";
            try
            {
                JobDataMap jobDataMap = context.JobDetail.JobDataMap;
                idAgendamento = jobDataMap.GetInt("idAgendamento");
                arquivo = new Arquivo();
                arquivoBO = new ArquivoBO(arquivo);
                if ((arquivo = arquivoDAO.Buscar(DateTime.Now.Date)) == null)
                {
                    int idEmissor = new EmissorDAO().Buscar("CBSS").IdEmissor;
                    arquivoBO.GerarArquivo(5, idEmissor, LAB5Utils.DataUtils.RetornaDataYYYYMMDD(DateTime.Now) + "_LQDINTELO.txt");
                }
                arquivoBO.Arquivo = arquivo;
                arquivoBO.Importar();
                message = "Liquidação Internacional Elo. Arquivo importado. ";
                sucesso = true;
                Logger.Info(this.ToString(), message, "QuartzJob");
                
            }
            catch (Exception ex)
            {
                message = "Erro ao executar importação automática do arquivo de Liquidação Nacional Elo. ";
                Logger.Warn(this.ToString(), message + ex.Message, "QuartzJob");
                sucesso = false;
                throw ex;
            }
            finally
            {
                new ExecucaoAgendamentoBO().SalvarExecucaoAgendamento(idAgendamento, DateTime.Now, message, sucesso);
            }
        }

        public void Start(int idAgendamento,string cronExpression)
        {
            JobDataMap jobMap = new JobDataMap();
            jobMap.Add("idAgendamento", idAgendamento);
            if (cronExpression != string.Empty)
                CDTScheduler.StartJobSchedule<LiquidacaoInternacionalEloJob>(jobMap,cronExpression, this.ToString() + idAgendamento.ToString(), "grp_" + this.ToString() + idAgendamento.ToString());
            else
                CDTScheduler.StartJobSchedule<LiquidacaoInternacionalEloJob>(jobMap,this.ToString() + idAgendamento.ToString(), "grp_" + this.ToString() + idAgendamento.ToString());
        }
    }
}
