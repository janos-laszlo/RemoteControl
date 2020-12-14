using Autofac;
using Domain.CommandFactories;
using Domain.Commands;
using Domain.Common.TaskScheduling;
using Domain.MessageReception;
using Domain.NightlyShutdown;
using System.ServiceProcess;
using WindowsLibrary.CommandFactories;
using WindowsLibrary.Controllers;
using WindowsLibrary.MessageReception;
using WindowsLibrary.NightlyShutdown;

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
            builder.RegisterType<JsonCommandFactory>().As<ITextCommandFactory>();
            builder.RegisterType<MessageReceptionist>().As<IMessageReceptionist>();
            builder.RegisterType<NightlyShutdownScheduler>().As<IShutdownScheduler>();
            builder.RegisterType<CmdLinePowerController>().As<IPowerController>();
            builder.RegisterType<CmdLineVolumeController>().As<IVolumeController>();
            builder.RegisterType<SystemInformation>().As<ISystemInformation>();
            builder.RegisterType<NightlyShutdownCalculator>().As<IShutdownCalculator>();
            builder.RegisterType<ParameterizedShutdownCommandFactory>().As<IShutdownCommandFactory>();
            builder.RegisterType<NightlyShutdownHistoryUpdater>().As<IShutdownHistoryUpdater>();
            builder.RegisterType<ShutdownHistoryStorage>().As<IShutdownHistoryStorage>().SingleInstance();
            builder.RegisterType<CommonTaskScheduler>().As<ITaskScheduler>().SingleInstance();
            return builder.Build();
        }
    }
}
