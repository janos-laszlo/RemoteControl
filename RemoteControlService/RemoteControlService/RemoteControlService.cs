using RemoteControlService.ReceiverDevice;
using System.ServiceProcess;
using System.Threading;

namespace RemoteControlService
{
    public partial class RemoteControlService : ServiceBase
    {
        readonly Thread t;

        public RemoteControlService()
        {
            InitializeComponent();
            t = new Thread(Setup);
        }

        public void OnDebug()
        {
            OnStart(new string[0]);
        }

        protected override void OnStart(string[] args)
        {
            t.Start();
        }

        protected override void OnStop()
        {
            t.Join();
        }

        private void Setup()
        {
            var receiver = new Receiver();
            receiver.Start();
        }
    }
}
