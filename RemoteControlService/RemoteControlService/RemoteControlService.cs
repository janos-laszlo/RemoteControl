using RemoteControlService.ReceiverDevice;

using System.ServiceProcess;

namespace RemoteControlService
{
    public partial class RemoteControlService : ServiceBase
    {
        readonly Receiver receiver;

        public RemoteControlService(Receiver receiver)
        {
            InitializeComponent();
            CanHandlePowerEvent = true;
            this.receiver = receiver;
        }

        public void OnDebug()
        {
            OnStart(new string[0]);
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            receiver.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();
            receiver?.Stop();
        }
        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            switch (powerStatus)
            {
                case PowerBroadcastStatus.ResumeSuspend:
                    receiver.Start();
                    break;
                case PowerBroadcastStatus.Suspend:
                    receiver.Stop();
                    break;
                default:
                    break;
            }

            return base.OnPowerEvent(powerStatus);
        }
    }
}
