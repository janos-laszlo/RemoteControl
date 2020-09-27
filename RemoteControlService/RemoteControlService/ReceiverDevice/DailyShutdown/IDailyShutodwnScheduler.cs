using System.Threading.Tasks;

namespace RemoteControlService.ReceiverDevice.DailyShutdown
{
    public interface IDailyShutodwnScheduler
    {
        Task ScheduleDailyShutdown();
        void UpdateShutdownHistory();
    }
}