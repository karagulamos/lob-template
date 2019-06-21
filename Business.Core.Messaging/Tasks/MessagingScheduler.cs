using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace Business.Core.Messaging.Tasks
{
    public class MessagingScheduler
    {
        public MessagingScheduler()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        public async Task Start()
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            foreach (var task in TaskFactory.All())
            {
                var detail = JobBuilder.Create(task.GetType()).Build();

                var trigger = TriggerBuilder.Create()
                       .WithSimpleSchedule(a => a.WithIntervalInSeconds(task.Interval.Seconds).RepeatForever())
                       .Build();

                await scheduler.ScheduleJob(detail, trigger);
            }
        }

        public async Task Stop()
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Shutdown();
        }
    }
}
