using System.Threading.Tasks;

namespace RemoteControlService.ReceiverDevice.NightlyShutdown
{
    public interface IShutdownScheduler
    {
        Task ScheduleShutdown();
    }
}