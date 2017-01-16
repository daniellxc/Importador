using CDT.Importacao.Data.Business;
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using CDT.Importacao.Data.Utils.Log;
using CDT.Importacao.Data.Utils.Quartz.Schedulers;
using LAB5;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Utils.Quartz.Jobs
{
    public class LiquidacaoNacionalEloJob : CDTJob
    {
        public void Execute(IJobExecutionContext context)
        {
            
            ArquivoDAO arquivoDAO = new ArquivoDAO();
            ArquivoBO arquivoBO = null;
            Arquivo arquivo = null;
            string message = "";
            int idAgendamento = 0;
            bool sucesso = false;
            try
            {
                JobDataMap jobDataMap = context.JobDetail.JobDataMap;
                idAgendamento = jobDataMap.GetInt("idAgendamento");
                arquivo = new Arquivo();
                arquivoBO = new ArquivoBO(arquivo);
                if ((arquivo = arquivoDAO.Buscar(DateTime.Now.Date)) == null)
                {
                    int idEmissor = new EmissorDAO().Buscar("CBSS").IdEmissor;
                    arquivoBO.GerarArquivo(1, idEmissor, LAB5Utils.DataUtils.RetornaDataYYYYMMDD(DateTime.Now) + "_LQDNACELO.txt");
                }
                arquivoBO.Arquivo = arquivo;
                arquivoBO.Importar();
                message = "Liquidação Nacional Elo. Arquivo importado.";
                sucesso = true;
                Logger.Info(this.ToString(),message, "QuartzJob");
            }
            catch(Exception ex)
            {
                message = "Erro ao executar importação automática do arquivo de Liquidação Nacional Elo. ";
                sucesso = false;
                Logger.Warn(this.ToString(), message + ex.Message, "QuartzJob");
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
                CDTScheduler.StartJobSchedule<LiquidacaoNacionalEloJob>(jobMap,cronExpression,this.ToString()+idAgendamento.ToString(),"grp_"+this.ToString()+idAgendamento.ToString());
            else
                CDTScheduler.StartJobSchedule<LiquidacaoNacionalEloJob>(jobMap,this.ToString() + idAgendamento.ToString(), "grp_" + this.ToString()+idAgendamento.ToString());
        }

    }
}
