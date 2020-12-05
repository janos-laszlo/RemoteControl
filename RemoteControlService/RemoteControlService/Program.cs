using Autofac;
using System.ServiceProcess;
using RemoteControlService.Common.TaskScheduling;
using RemoteControlService.Commands;
using RemoteControlService.NightlyShutdown;
using RemoteControlService.MessageReception;
using RemoteControlService.Controllers;
using RemoteControlService.Commands.CommandFactories;

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
            builder.RegisterType<JsonCommandFactory>();
            builder.RegisterType<MessageReceptionist>().As<IMessageReceptionist>();
            builder.RegisterType<NightlyShutdownScheduler>().As<IShutdownScheduler>();
            builder.RegisterType<ShutdownHistoryStorage>().As<IShutdownHistoryStorage>();
            builder.RegisterType<CmdLinePowerController>().As<IPowerController>();
            builder.RegisterType<CmdLineVolumeController>().As<IVolumeController>();
            builder.RegisterType<SystemInformation>().As<ISystemInformation>();
            builder.RegisterType<NightlyShutdownCalculator>().As<IShutdownCalculator>();
            builder.RegisterType<CommonTaskScheduler>().As<ITaskScheduler>();
            builder.RegisterType<ParameterizedShutdownCommandFactory>().As<IShutdownCommandFactory>();
            return builder.Build();
        }
    }
}
