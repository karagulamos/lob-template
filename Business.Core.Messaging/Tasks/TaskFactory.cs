using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Core.Messaging.Tasks
{
    public enum TaskType { Email = 1, Sms = 2 }

    public class TaskFactory
    {
        private static readonly Lazy<IDictionary<TaskType, Func<ITimedJob>>> LazyTasks;

        static TaskFactory()
        {
            var tasks = new Dictionary<TaskType, Func<ITimedJob>>
            {
                {TaskType.Email, () => new EmailSender()},
                //{TaskType.Sms, () => new SmsSender()}
            };

            LazyTasks = new Lazy<IDictionary<TaskType, Func<ITimedJob>>>(() => tasks);
        }

        public static ITimedJob Get(TaskType type)
        {
            if (!LazyTasks.Value.ContainsKey(type))
                throw new ArgumentException(type.ToString());

            return LazyTasks.Value[type].Invoke();
        }

        public static IEnumerable<ITimedJob> All()
        {
            return LazyTasks.Value.Values.Select(task => task.Invoke());
        }
    }
}
