using System;

namespace RemoteControlService.Common.TaskScheduling
{
    public interface ITaskScheduler
    {
        void ScheduleTask(Action task, DateTime executeAt);
    }
}