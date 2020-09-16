using RemoteControlService.ReceiverDevice.Commands;
using System;

namespace RemoteControlService.UniTests
{
    internal class CmdLinePowerControllerMock : IPowerController
    {
        public void CancelShutdown()
        {
            Console.WriteLine("Canceling shutdown");
        }

        public void Hibernate()
        {
            Console.WriteLine($"Going to sleep at {DateTime.Now}");
        }

        public void ScheduleShutdown(int seconds)
        {
            Console.WriteLine($"Shuting down after {seconds} seconds");
        }
    }
}