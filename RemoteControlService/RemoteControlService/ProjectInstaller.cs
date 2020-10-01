using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;

namespace RemoteControlService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        const string ServiceName = "Remote Control Service";

        public ProjectInstaller()
        {
            InitializeComponent();
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);

            string path = Path.GetDirectoryName(Context.Parameters["assemblypath"]);
            OpenFirewallForProgram(Path.Combine(path, "RemoteControlService.exe"), ServiceName);
            AddReadWriteAccessForLocalService(Path.Combine(path, "shutdownHistory.json"));
        }

        protected override void OnAfterUninstall(IDictionary savedState)
        {
            base.OnAfterUninstall(savedState);

            string path = Path.GetDirectoryName(Context.Parameters["assemblypath"]);
            RemoveFirewallRuleForProgram(Path.Combine(path, "RemoteControlService.exe"), ServiceName);
        }

        private void RemoveFirewallRuleForProgram(string exeFullPath, string displayName)
        {
            var proc = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = string.Format("advfirewall firewall delete rule name = \"{0}\" program = \"{1}\"", displayName, exeFullPath),
                    WindowStyle = ProcessWindowStyle.Hidden
                });

            proc.WaitForExit();
        }

        private static void OpenFirewallForProgram(string exeFullPath, string displayName)
        {
            var proc = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = string.Format("advfirewall firewall add rule name = \"{0}\" dir=in action=allow program=\"{1}\" enable = yes profile = PUBLIC", displayName, exeFullPath),
                    WindowStyle = ProcessWindowStyle.Hidden
                });

            proc.WaitForExit();
        }

        private void AddReadWriteAccessForLocalService(string path)
        {
            var proc = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "icacls",
                    Arguments = $"\"{path}\" /grant \"LOCAL SERVICE\":(GR,GW)",
                    WindowStyle = ProcessWindowStyle.Hidden
                });

            proc.WaitForExit();
        }

        private void RemoteControlServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void RemoteControlServiceProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
