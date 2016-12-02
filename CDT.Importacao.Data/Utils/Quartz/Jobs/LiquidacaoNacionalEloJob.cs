using CDT.Importacao.Data.Business;
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Utils.Quartz.Jobs
{
    public class LiquidacaoNacionalEloJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ArquivoDAO arquivoDAO = new ArquivoDAO();
            ArquivoBO arquivoBO = null;
            Arquivo arquivo = null;
            try
            {
                JobDataMap jobDataMap = context.JobDetail.JobDataMap;

                arquivo = new Arquivo();
                arquivoBO = new ArquivoBO(arquivo);
                if ((arquivo = arquivoDAO.Buscar(DateTime.Now.Date)) == null)
                {
                    
                    string idLayout = jobDataMap.GetString("layout");
                    string idEmissor = jobDataMap.GetString("emissor");
                    arquivoBO.GerarArquivo(int.Parse(idLayout), int.Parse(idEmissor));
                }
                arquivoBO.Arquivo = arquivo;
                arquivoBO.Importar();

            }
            catch(Exception ex)
            {
                //loggar
                throw ex;
            }
        }
    }
}
