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
     
        static IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

        public static void StartJobSchedule<T>(string jobName, string groupName) where T : IJob
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();

            if (scheduler.CheckExists(new JobKey(jobName, groupName))) throw new Exception("Já existe job agendado.");

            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity(jobName, groupName)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()

                .WithIdentity("trg_"+jobName, groupName)
                .StartNow()
                .WithPriority(1)
                .Build();
    
            scheduler.ScheduleJob(job, trigger);
        }

        public static void StartJobSchedule<T>(string cronExpression, string jobName, string groupName) where T : IJob
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            
            scheduler.Start();

            if (scheduler.CheckExists(new JobKey(jobName, groupName)))  throw new Exception("Já existe job agendado.");

                IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity(jobName, groupName)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()

                .WithIdentity("trg_"+jobName, groupName)
                .WithPriority(1)
                .WithCronSchedule(cronExpression)
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }

        public static void StartJobSchedule<T>(JobDataMap jobmap, string cronExpression, string jobName, string groupName) where T : IJob
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();

            if (scheduler.CheckExists(new JobKey(jobName, groupName))) throw new Exception("Já existe job agendado.");

            IJobDetail job = JobBuilder.Create<T>()
            .WithIdentity(jobName, groupName)
            .SetJobData(jobmap)
            .Build();

            ITrigger trigger = TriggerBuilder.Create()

                .WithIdentity("trg_" + jobName, groupName)
                .WithPriority(1)
                .WithCronSchedule(cronExpression)
                .Build();

            scheduler.ScheduleJob(job, trigger);

        }

        public static void StartJobSchedule<T>(JobDataMap jobMap, string jobName, string groupName) where T : IJob
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();

            if (scheduler.CheckExists(new JobKey(jobName, groupName))) throw new Exception("Já existe job agendado.");

            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity(jobName, groupName)
                .SetJobData(jobMap)
                .Build();

            ITrigger trigger = TriggerBuilder.Create()

                .WithIdentity("trg_" + jobName, groupName)
                .StartNow()
                .WithPriority(1)
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }

        //public static void StartJobSchedule<T>(IJobDetail job, string cronExpression) where T : IJob
        //{
        //    IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

        //    scheduler.Start();



        //    ITrigger trigger = TriggerBuilder.Create()

        //        //.WithIdentity("LiquidacaoNacionalEloJob", "JobGroup")
        //        //.WithCronSchedule("0/10 * * 1/1 * ? *")
        //        .WithIdentity("job1", "group1")
        //        .StartAt(DateTime.Now)
        //        .WithCronSchedule(cronExpression)
        //        .WithPriority(1)
        //        .Build();


        //    scheduler.ScheduleJob(job, trigger);
        //}


        public static IList<IJobExecutionContext> GetCurrentJobs()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            return scheduler.GetCurrentlyExecutingJobs();
        }

        public static void DeleteJob(string jobName, string groupName)
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            if (scheduler.CheckExists(new JobKey(jobName, groupName)))
                scheduler.DeleteJob(new JobKey(jobName, groupName));
        }

        public static bool JobExists(string jobName, string groupName)
        {
            return scheduler.CheckExists(new JobKey(jobName, groupName));
        }

        public static DateTime NextExecutionTime(string jobName, string groupName)
        {
            ITrigger trigger = scheduler.GetTrigger(new TriggerKey("trg_"+jobName, groupName));
            if(trigger != null)
            {
                var time = trigger.GetNextFireTimeUtc();
                return  TimeZone.CurrentTimeZone.ToLocalTime(time.Value.DateTime);
            }
            return DateTime.MinValue;
        }
   
    }
}
