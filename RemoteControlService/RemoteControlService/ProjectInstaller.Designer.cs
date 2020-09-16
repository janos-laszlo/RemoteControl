namespace RemoteControlService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.RemoteControlServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.RemoteControlServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // RemoteControlServiceProcessInstaller
            // 
            this.RemoteControlServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.RemoteControlServiceProcessInstaller.Password = null;
            this.RemoteControlServiceProcessInstaller.Username = null;
            this.RemoteControlServiceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.RemoteControlServiceProcessInstaller_AfterInstall);
            // 
            // RemoteControlServiceInstaller
            // 
            this.RemoteControlServiceInstaller.Description = "Execute actions on this device, like shuting it down, changing the volume. These " +
    "action requests are received from a remote device.";
            this.RemoteControlServiceInstaller.DisplayName = "Remote Control Service";
            this.RemoteControlServiceInstaller.ServiceName = "RemoteControlService";
            this.RemoteControlServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.RemoteControlServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.RemoteControlServiceInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.RemoteControlServiceProcessInstaller,
            this.RemoteControlServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller RemoteControlServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller RemoteControlServiceInstaller;
    }
}