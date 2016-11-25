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
    public class ConciliacaoJobScheduler
    {
        public static void Start()

        {

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            scheduler.Start();

            IJobDetail job = JobBuilder.Create<ConciliacaoJob>().Build();

            ITrigger trigger = TriggerBuilder.Create()

                .WithIdentity("ConciliacaoJob", "JobGroup")

                .WithCronSchedule("0/10 * * 1/1 * ? *")

                .StartAt(DateTime.UtcNow)

                .WithPriority(1)

                .Build();

            scheduler.ScheduleJob(job, trigger);

        }
    }
}
