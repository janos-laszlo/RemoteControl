using Domain;
using Domain.Commands;
using Domain.Controllers;
using System;

namespace ReceiverWinFormsApp
{
    public class MainPageViewModel
    {
        private readonly Receiver receiver;
        private readonly IPowerController powerController;

        public bool IsRunning { get; private set; } = false;

        public MainPageViewModel(Receiver receiver, IPowerController powerController)
        {
            this.receiver = receiver;
            this.powerController = powerController;
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

        internal void CancelShutdown()
        {
            new CancelShutdownCommand(
                powerController,
                showNotification: true).Execute();
        }
    }
}
