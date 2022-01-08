using Domain;
using Domain.CommandFactories;
using Domain.Common.TaskScheduling;
using Domain.Controllers;
using Domain.MessageReception;
using Domain.NightlyShutdown;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using WindowsLibrary.CommandFactories;
using WindowsLibrary.Controllers;
using WindowsLibrary.MessageReception;
using WindowsLibrary.NightlyShutdown;

namespace ReceiverWinFormsApp
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var services = new ServiceCollection();
            ConfigureServices(services);
            using var serviceProvider = services.BuildServiceProvider();
            var mainPage = serviceProvider.GetRequiredService<MainPage>();
            Application.Run(mainPage);
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                TextWriterTraceListener listener = new(@"C:\Users\Public\Logs\shutdown history.txt");
                listener.TraceOutputOptions = TraceOptions.DateTime;

                SourceSwitch sourceSwitch = new("ReceiverWinFormsApp", "ReceiverWinFormsApp");
                sourceSwitch.Level = SourceLevels.Information;
                Trace.AutoFlush = true;
                builder.AddTraceSource(
                    sourceSwitch,
                    listener);
            });

            services.AddTransient<MainPage>();
            services.AddTransient<MainPageViewModel>();
            services.AddTransient<Receiver>();
            services.AddTransient<ITextCommandFactory, JsonCommandFactory>();
            services.AddTransient<IMessageReceptionist, MessageReceptionist>();
            services.AddTransient<IShutdownScheduler, NightlyShutdownScheduler>();
            services.AddTransient<IPowerController, CmdLinePowerController>();
            services.AddTransient<IVolumeController, CmdLineVolumeController>();
            services.AddTransient<ISystemInformation, WindowsLibrary.NightlyShutdown.SystemInformation>();
            services.AddTransient<IShutdownCalculator, NightlyShutdownCalculator>();
            services.AddTransient<IShutdownCommandFactory, ParameterizedShutdownCommandFactory>();
            services.AddTransient<IShutdownHistoryUpdater, NightlyShutdownHistoryUpdater>();
            services.AddSingleton<IShutdownHistoryStorage, ShutdownHistoryStorage>();
            services.AddSingleton<ITaskScheduler, CommonTaskScheduler>();
            services.AddTransient<CommandProcessor>();
        }
    }
}
