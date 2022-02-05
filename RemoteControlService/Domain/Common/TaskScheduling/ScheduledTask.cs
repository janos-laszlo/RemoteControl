using System;

namespace Domain.Common.TaskScheduling
{
    public class ScheduledTask
    {
        public static readonly Action EmptyAction = () => { };

        public ScheduledTask(Action task, DateTime executeAt)
        {
            Action = task;
            ExecuteAt = executeAt;
        }
        public Action Action { get; private set; }
        public DateTime ExecuteAt { get; private set; }
        public void Cancel()
        {
            Action = EmptyAction;
        }
    }
}
