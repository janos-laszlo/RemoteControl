using Domain.Commands.Arguments;
using Domain.Common.TaskScheduling;
using Domain.Controllers;
using System;
using System.Management;
using System.Windows.Forms;

namespace WindowsLibrary.Controllers
{
    public class WindowsFormsPowerController : IPowerController
    {
        private readonly ITaskScheduler taskScheduler;
        private ScheduledTask task;

        public WindowsFormsPowerController(ITaskScheduler taskScheduler)
        {
            this.taskScheduler = taskScheduler;
        }

        public void CancelShutdown()
        {
            task?.Cancel();
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

            var executeAt = DateTime.Now.AddSeconds(arguments.Seconds);
            CancelShutdown();
            task = new ScheduledTask(Shutdown, executeAt);
            taskScheduler.ScheduleTask(task);
        }

        void Shutdown()
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
            task != null && task.Action != ScheduledTask.EmptyAction;
    }
}
