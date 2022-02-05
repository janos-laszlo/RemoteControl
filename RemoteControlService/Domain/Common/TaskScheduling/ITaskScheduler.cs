using System;

namespace Domain.Common.TaskScheduling
{
    public interface ITaskScheduler
    {
        void ScheduleTask(ScheduledTask scheduledTask);
        void Start();
        void Stop();
    }
}