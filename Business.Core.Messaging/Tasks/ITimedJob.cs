using System;
using Quartz;

namespace Business.Core.Messaging.Tasks
{
    public interface ITimedJob : IJob
    {
        TimeSpan Interval { get; }
    }
}