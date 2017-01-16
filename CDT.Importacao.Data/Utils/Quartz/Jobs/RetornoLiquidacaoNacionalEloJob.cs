using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using CDT.Importacao.Data.Utils.Quartz.Schedulers;
using CDT.Importacao.Data.Model;
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Business;
using LAB5;
using CDT.Importacao.Data.Utils.Log;

namespace CDT.Importacao.Data.Utils.Quartz.Jobs
{
    public class RetornoLiquidacaoNacionalEloJob : CDTJob
    {
        int idAgendamento = 0;
        bool sucesso = false;
        string message;

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Layout layout = new LayoutDAO().Buscar("ELO - Liquidação Nacional");
                if (layout != null)
                {
                    JobDataMap jobDataMap = context.JobDetail.JobDataMap;
                    idAgendamento = jobDataMap.GetInt("idAgendamento");
                    string nomeArquivo = LAB5Utils.DataUtils.RetornaDataYYYYMMDD(DateTime.Now) + "_LQD_NACIONAL_RETORNO.txt";
                    Arquivo arquivo = new ArquivoDAO().BuscarPorLayout(layout.IdLayout).OrderByDescending(d => d.DataImportacao).First();
                    new ArquivoRetornoElo(arquivo).MontarArquivoRetorno(layout.DiretorioArquivo, nomeArquivo);
                    message = "ELO - LIQUIDACAO NACIONAL - Arquivo de retorno gerado com sucesso. ";
                    sucesso = true;
                    Logger.Info(this.ToString(), message, "QuartzJob");
                }
                else throw new Exception("Layout nao encontrado.");
            }catch(Exception ex)
            {
                message = "ELO - LIQUIDACAO NACIONAL - Erro ao executar job para geração do arquivo retorno de liquidacao nacional elo. " + ex.Message; ;
                sucesso = false;
                Logger.Warn(this.ToString(), message, "QuartzJob");
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
                CDTScheduler.StartJobSchedule<RetornoLiquidacaoNacionalEloJob>(jobMap,cronExpression, this.ToString() + idAgendamento.ToString(), "grp_" + this.ToString() + idAgendamento.ToString());
            else
                CDTScheduler.StartJobSchedule<RetornoLiquidacaoNacionalEloJob>(jobMap,this.ToString() + idAgendamento.ToString(), "grp_" + this.ToString() + idAgendamento.ToString());
        }
    }
}
