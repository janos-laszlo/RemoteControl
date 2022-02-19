using Domain.Common.DataStructures;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace Domain.Common.TaskScheduling
{
    public class CommonTaskScheduler : ITaskScheduler
    {
        private readonly ConcurrentPriorityQueue<ScheduledTask> tasks = new((s1, s2) => s1.ExecuteAt < s2.ExecuteAt);
        private Timer? timer;
        private bool isRunning;

        public void Start()
        {
            if (isRunning)
            {
                return;
            }

            if (timer is null)
            {
                timer = new Timer();
                timer.Elapsed += ExecuteDueTasks;
            }

            SetTimerIntervalToPriorityQueueTop();
            timer.Start();
            isRunning = true;
        }

        public void Stop()
        {
            if (!isRunning)
            {
                return;
            }

            timer?.Stop();
            isRunning = false;
        }

        public void ScheduleTask(ScheduledTask scheduledTask)
        {
            tasks.Enqueue(scheduledTask);
            SetTimerIntervalToPriorityQueueTop();
        }

        private void ExecuteDueTasks(object sender, ElapsedEventArgs e)
        {
            if (tasks.TryDequeue(out ScheduledTask? task))
            {
                Task.Run(task!.Action);
                SetTimerIntervalToPriorityQueueTop();
            }
        }

        private void SetTimerIntervalToPriorityQueueTop()
        {
            if (timer is null)
            {
                return;
            }

            if (tasks.TryPeek(out ScheduledTask? topTask))
            {
                timer.Interval = Math.Max(
                    1,
                    (topTask!.ExecuteAt - DateTime.Now).TotalMilliseconds);
            }
            else
            {
                timer.Interval = int.MaxValue;
            }
        }
    }
}
