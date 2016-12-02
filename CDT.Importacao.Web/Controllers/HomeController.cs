using CDT.Importacao.Data.Utils.Quartz.Jobs;
using CDT.Importacao.Data.Utils.Quartz.Schedulers;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CDT.Importacao.Web.Controllers
{
   [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            /*INICIANDO SCHEDULERS*/
            //IJobDetail detail = JobBuilder.Create<LiquidacaoNacionalEloJob>()
            //    .WithIdentity("myJob","jobGroup")
            //    .UsingJobData("layout", "1")
            //    .UsingJobData("emissor","85")

            //    .Build();
            try
            {
                IJobDetail job = JobBuilder.Create<LiquidacaoNacionalEloJob>()
                     .WithIdentity("myJob", "group1")
                     .UsingJobData("emissor", "85")
                     .UsingJobData("layout", "1")
                     .Build();
                CDTScheduler.StartJobSchedule<LiquidacaoNacionalEloJob>(job, "0/10 * * 1/1 * ? *");
            }catch(Exception ex)
            {
                Alert(ex.Message);
            }
             


            //ISchedulerFactory schedFact = new StdSchedulerFactory();

            //// get a scheduler
            //IScheduler sched = schedFact.GetScheduler();
            //sched.Start();

            //// define the job and tie it to our HelloJob class
            //IJobDetail job = JobBuilder.Create<LiquidacaoNacionalEloJob>()
            //    .WithIdentity("myJob", "group1")
            //     .UsingJobData("layout", "1")
            //    .UsingJobData("emissor","85")
            //    .Build();

            //// Trigger the job to run now, and then every 40 seconds
            //ITrigger trigger = TriggerBuilder.Create()
            //  .WithIdentity("myTrigger", "group1")
            //  .StartNow()     
            //  .Build();

            //sched.ScheduleJob(job, trigger);

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

       
    }
}