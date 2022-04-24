using Domain;
using Domain.Commands.Arguments;
using Domain.Common.TaskScheduling;
using Domain.Controllers;
using System;
using System.Management;
using System.Windows.Forms;

namespace ReceiverWinFormsApp.Controllers
{
    public class WindowsPowerController : IPowerController
    {
        private readonly ITaskScheduler taskScheduler;
        private readonly INotifier notifier;

        private ScheduledTask shutdownTask;
        private ScheduledTask notificationTask;

        public WindowsPowerController(ITaskScheduler taskScheduler, INotifier notifier)
        {
            this.taskScheduler = taskScheduler;
            this.notifier = notifier;
        }

        public DateTime? NextShutdownDateTime { get; private set; }

        public void CancelShutdown(bool showNotification)
        {
            shutdownTask?.Cancel();
            notificationTask?.Cancel();
            NextShutdownDateTime = null;
            if (showNotification)
            {
                notifier.Notify("Shutdown canceled");
            }
        }

        public void Hibernate()
        {
            Application.SetSuspendState(PowerState.Hibernate, force: false, disableWakeEvent: true);
        }

        public void ScheduleShutdown(ShutdownArgs arguments)
        {
            if (!arguments.OverrideExistingShutdown && ScheduledShutdownExists())
            {
                return;
            }

            CancelShutdown(false);
            if (arguments.ShowNotification)
            {
                notificationTask = new ScheduledTask(
                    () => notifier.Notify($"System will shutdown in 10 minutes"),
                    arguments.DateTime.AddMinutes(-10));
                taskScheduler.ScheduleTask(notificationTask);
            }

            shutdownTask = new ScheduledTask(Shutdown, arguments.DateTime);
            taskScheduler.ScheduleTask(shutdownTask);
            NextShutdownDateTime = arguments.DateTime;
        }

        static void Shutdown()
        {
            var mcWin32 = new ManagementClass("Win32_OperatingSystem");
            mcWin32.Get();

            // You can't shutdown without security privileges
            mcWin32.Scope.Options.EnablePrivileges = true;
            ManagementBaseObject mboShutdownParams =
                     mcWin32.GetMethodParameters("Win32Shutdown");

            // Flag 1 means we want to shut down the system. Use "2" to reboot.
            mboShutdownParams["Flags"] = "1";
            mboShutdownParams["Reserved"] = "0";
            foreach (ManagementObject manObj in mcWin32.GetInstances())
            {
                _ = manObj.InvokeMethod(
                    "Win32Shutdown",
                    mboShutdownParams,
                    null);
            }
        }

        private bool ScheduledShutdownExists() =>
            shutdownTask != null && shutdownTask.Action != ScheduledTask.EmptyAction;
    }
}
