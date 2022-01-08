using Domain;

namespace ReceiverWinFormsApp
{
    public class MainPageViewModel
    {
        private readonly Receiver receiver;
        
        public bool IsRunning { get; private set; } = false;

        public MainPageViewModel(Receiver receiver)
        {
            this.receiver = receiver;
        }

        public void StartReceiver()
        {
            receiver.Start();
            IsRunning = true;
        }

        internal void StopReceiver()
        {
            receiver.Stop();
            IsRunning = false;
        }
    }
}
