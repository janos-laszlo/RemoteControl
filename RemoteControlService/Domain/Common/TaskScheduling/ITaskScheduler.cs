using System;

namespace Domain.Common.TaskScheduling
{
    public interface ITaskScheduler
    {
        void ScheduleTask(Action task, DateTime executeAt);
    }
}