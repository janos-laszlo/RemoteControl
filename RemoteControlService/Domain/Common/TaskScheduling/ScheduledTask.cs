using System;

namespace Domain.Common.TaskScheduling
{
    public class ScheduledTask
    {
        public ScheduledTask(Action task, DateTime executeAt)
        {
            Task = task;
            ExecuteAt = executeAt;
        }
        public Action Task { get; private set; }
        public DateTime ExecuteAt { get; private set; }
        public void Cancel()
        {
            Task = () => { };
        }
    }
}
