using System;

namespace Domain.Commands
{
    public class ShutdownCommand : ICommand
    {
        private readonly int seconds;
        private readonly IPowerController powerController;
        private readonly bool overrideScheduledShutdown = true;

        public ShutdownCommand(int seconds, IPowerController powerController)
        {
            if (seconds < 0) throw new ArgumentException($"seconds expected to be a positive integer, but was {seconds}");
            this.seconds = seconds;
            this.powerController = powerController;
        }

        public ShutdownCommand(int seconds, IPowerController powerController, bool overrideScheduledShutdown) : this(seconds, powerController)
        {
            this.overrideScheduledShutdown = overrideScheduledShutdown;
        }

        public void Execute()
        {
            powerController.ScheduleShutdown(seconds, overrideScheduledShutdown);
        }
    }
}
