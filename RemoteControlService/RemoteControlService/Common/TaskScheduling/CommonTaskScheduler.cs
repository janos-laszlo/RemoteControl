using RemoteControlService.Common.DataStructures;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace RemoteControlService.Common.TaskScheduling
{
    public class CommonTaskScheduler : ITaskScheduler
    {
        private readonly ConcurrentPriorityQueue<ScheduledTask> tasks = new ConcurrentPriorityQueue<ScheduledTask>((s1, s2) => s1.ExecuteAt < s2.ExecuteAt);
        private readonly Timer timer = new Timer();

        public CommonTaskScheduler()
        {
            timer.Elapsed += ExecuteDueTasks;
            SetTimerIntervalToPriorityQueueTop();
            timer.Start();
        }

        public void ScheduleTask(Action task, DateTime executeAt)
        {
            tasks.Enqueue(new ScheduledTask(task, executeAt));
            SetTimerIntervalToPriorityQueueTop();
        }

        private void ExecuteDueTasks(object sender, ElapsedEventArgs e)
        {
            if (tasks.TryDequeue(out ScheduledTask task))
            {
                Task.Run(task.Task);
                SetTimerIntervalToPriorityQueueTop();
            }
        }

        private void SetTimerIntervalToPriorityQueueTop()
        {
            if (tasks.TryPeek(out ScheduledTask topTask))
            {
                timer.Interval = Math.Max(1, (topTask.ExecuteAt - DateTime.Now).TotalMilliseconds);
            }
            else
            {
                timer.Interval = int.MaxValue;
            }
        }
    }

    class ScheduledTask
    {
        public ScheduledTask(Action task, DateTime executeAt)
        {
            Task = task;
            ExecuteAt = executeAt;
        }
        public Action Task { get; set; }
        public DateTime ExecuteAt { get; set; }
    }
}
