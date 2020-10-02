using System;

namespace RemoteControlService.ReceiverDevice.Commands
{
    class ShutdownCommand : ICommand
    {
        private readonly int seconds;
        private readonly IPowerController powerController;

        public ShutdownCommand(int seconds, IPowerController powerController)
        {
            if (seconds < 0) throw new ArgumentException($"seconds expected to be a positive integer, but was {seconds}");
            this.seconds = seconds;
            this.powerController = powerController;
        }

        public void Execute()
        {
            powerController.ScheduleShutdown(seconds, overrideScheduledShutdown: true);
        }
    }
}
