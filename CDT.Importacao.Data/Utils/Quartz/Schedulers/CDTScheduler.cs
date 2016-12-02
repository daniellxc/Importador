using CDT.Importacao.Data.Utils.Quartz.Jobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Utils.Quartz.Schedulers
{
    public  class CDTScheduler
    {
     
        public static void StartJobSchedule<T>() where T : IJob
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();


            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity("myJob", "group1")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()

                .WithIdentity("myTrigger", "group1")
                .StartNow()
                .WithPriority(1)
                .Build();
    
            scheduler.ScheduleJob(job, trigger);
        }

        public static void StartJobSchedule<T>(string cronExpression) where T : IJob
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();


            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity("myJob", "group1")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()

                .WithIdentity("myTrigger", "group1")
                .StartNow()
                .WithPriority(1)
                .Build();
            if (!cronExpression.Equals(string.Empty))
                trigger.GetTriggerBuilder().WithCronSchedule(cronExpression);

            scheduler.ScheduleJob(job, trigger);
        }

        public static void StartJobSchedule<T>(IJobDetail job, string cronExpression) where T : IJob
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();
            
            ITrigger trigger = TriggerBuilder.Create()

                //.WithIdentity("LiquidacaoNacionalEloJob", "JobGroup")
                //.WithCronSchedule("0/10 * * 1/1 * ? *")
                .WithIdentity("myTrigger", "group1")
                .StartAt(DateTime.Now)
                .WithCronSchedule(cronExpression)
                .WithPriority(1)
                .Build();


            scheduler.ScheduleJob(job, trigger);
        }
    }
}
