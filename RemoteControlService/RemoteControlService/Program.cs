using Autofac;
using RemoteControlService.ReceiverDevice;
using RemoteControlService.ReceiverDevice.Commands;
using RemoteControlService.ReceiverDevice.Controllers;
using RemoteControlService.ReceiverDevice.NightlyShutdown;
using RemoteControlService.ReceiverDevice.MessageReception;
using System.ServiceProcess;

namespace RemoteControlService
{
    static class Program
    {
        private static IContainer Container { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Container = CreateIoCContainer();

            using (var scope = Container.BeginLifetimeScope())
            {
                RemoteControlService rcs = scope.Resolve<RemoteControlService>();
#if DEBUG
                rcs.OnDebug();
#else
                ServiceBase[] ServicesToRun = new ServiceBase[] { rcs };
                ServiceBase.Run(ServicesToRun);
#endif
            }
        }

        private static IContainer CreateIoCContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<RemoteControlService>();
            builder.RegisterType<Receiver>();
            builder.RegisterType<CommandFactory>();
            builder.RegisterType<MessageReceptionist>().As<IMessageReceptionist>();
            builder.RegisterType<NightlyShutdownScheduler>().As<IShutdownScheduler>();
            builder.RegisterType<ShutdownHistoryStorage>().As<IShutdownHistoryStorage>();
            builder.RegisterType<CmdLinePowerController>().As<IPowerController>();
            builder.RegisterType<CmdLineVolumeController>().As<IVolumeController>();
            builder.RegisterType<SystemInformation>().As<ISystemInformation>();
            builder.RegisterType<NightlyShutdownCalculator>().As<IShutdownCalculator>();
            return builder.Build();
        }
    }
}
