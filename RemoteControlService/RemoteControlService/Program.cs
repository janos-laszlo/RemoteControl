using System.ServiceProcess;

namespace RemoteControlService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            RemoteControlService rms = new RemoteControlService();
            rms.OnDebug();
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new RemoteControlService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
