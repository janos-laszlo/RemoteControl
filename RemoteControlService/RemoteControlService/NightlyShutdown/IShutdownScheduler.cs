using System.Threading.Tasks;

namespace RemoteControlService.NightlyShutdown
{
    public interface IShutdownScheduler
    {
        void ScheduleShutdown();
    }
}