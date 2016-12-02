using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Utils.Quartz.Schedulers
{
    public abstract class AbstractScheduler<T> where T : IJob
    {
        public void Schedule()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();


            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity("myJob", "group1")
               
                .Build();

            ITrigger trigger = TriggerBuilder.Create()

                //.WithIdentity("LiquidacaoNacionalEloJob", "JobGroup")

                //.WithCronSchedule("0/10 * * 1/1 * ? *")
                .WithIdentity("myTrigger", "group1")
                .StartNow()
                .WithPriority(1)
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}
