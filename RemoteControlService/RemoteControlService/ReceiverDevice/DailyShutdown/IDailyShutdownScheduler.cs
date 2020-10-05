using System.Threading.Tasks;

namespace RemoteControlService.ReceiverDevice.DailyShutdown
{
    public interface IDailyShutdownScheduler
    {
        Task ScheduleDailyShutdown();
    }
}