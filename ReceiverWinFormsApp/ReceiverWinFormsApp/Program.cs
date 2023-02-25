using DataContracts;
using Domain;
using Domain.CommandFactories;
using Domain.Common.TaskScheduling;
using Domain.Controllers;
using Domain.MessageReception;
using Domain.NightlyShutdown;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReceiverWinFormsApp.CommandFactories;
using ReceiverWinFormsApp.Controllers;
using ReceiverWinFormsApp.MessageReception;
using ReceiverWinFormsApp.NightlyShutdown;
using ReceiverWinFormsApp.Notification;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace ReceiverWinFormsApp
{
    static class Program
    {

        private static Locations Locations;


        [STAThread]
        static void Main()
        {

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            InitAppData();

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
                TextWriterTraceListener listener = new(Locations.LogFilePath);
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
            services.AddSingleton<IPowerController, WindowsPowerController>();
            services.AddTransient<IVolumeController, NAudioVolumeController>();
            services.AddTransient<ISystemInformation, NightlyShutdown.SystemInformation>();
            services.AddTransient<IShutdownCalculator, NightlyShutdownCalculator>();
            services.AddTransient<IShutdownCommandFactory, ParameterizedShutdownCommandFactory>();
            services.AddTransient<IShutdownHistoryUpdater, NightlyShutdownHistoryUpdater>();
            services.AddSingleton<IShutdownHistoryStorage, ShutdownHistoryStorage>();
            services.AddSingleton<ITaskScheduler, CommonTaskScheduler>();
            services.AddSingleton<INotifier, WindowsNotifier>();
            services.AddTransient<CommandProcessor>();
            services.AddSingleton(Locations);
        }

        private static void InitAppData()
        {
            var dto = JsonSerializer.Deserialize<LocationPaths>(File.ReadAllText(@"Resources\LocationPaths.json"));

            Locations = new(
#if DEBUG
            applicationDataFolder: ".",
#else
            applicationDataFolder: Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), dto.ApplicationDataFolderName),
#endif
            logFileName: dto.LogFileName,
            shutdownHistoryFileName: dto.ShutdownHistoryFileName);

            Directory.CreateDirectory(Locations.ApplicationDataFolder);


            if (!File.Exists(Locations.LogFilePath))
            {
                File.WriteAllText(Locations.LogFilePath, string.Empty);
            }
        }
    }
}
