namespace HostLiveWindowsService
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
            this.HostLiveProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.HostLive = new System.ServiceProcess.ServiceInstaller();
            // 
            // HostLiveProcessInstaller1
            // 
            this.HostLiveProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.HostLiveProcessInstaller1.Password = null;
            this.HostLiveProcessInstaller1.Username = null;
            // 
            // HostLive
            // 
            this.HostLive.Description = "This is HostLive service when system boots up we wil trigger this service.";
            this.HostLive.DisplayName = "HostLive";
            this.HostLive.ServiceName = "HostLive";
            this.HostLive.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.HostLiveProcessInstaller1,
            this.HostLive});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller HostLiveProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller HostLive;
    }
}