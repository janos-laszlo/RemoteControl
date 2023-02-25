using Domain;
using Domain.Controllers;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace ReceiverWinFormsApp
{
    public partial class MainPage : Form
    {
        private readonly MainPageViewModel viewModel;

        public MainPage(MainPageViewModel viewModel, IPowerController powerController, INotifier notifier)
        {
            InitializeComponent();
            SystemEvents.PowerModeChanged += OnPowerChanged;
            this.viewModel = viewModel;
            viewModel.StartReceiver();
            powerController.NextShutdownChanged += args => HandleEventOnThisForm(() => OnNextShutdownChanged(args));
            notifier.OnActivated += () => HandleEventOnThisForm(ShowMyself);
            UpdateButtonText();
        }

        private void HandleEventOnThisForm(Action action)
        {
            if (this.InvokeRequired)
                this.Invoke(HandleEventOnThisForm, action);
            else
                action();
        }

        private void OnNextShutdownChanged(DateTime? nextShutdownDateTime)
        {
            nextShutdownLabel.Text = nextShutdownDateTime is null
                ? "Shutdown not scheduled"
                : $"Shutting down at {nextShutdownDateTime.Value:HH:mm}";
        }

        private void OnPowerChanged(object s, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    viewModel.StartReceiver();
                    break;
                case PowerModes.Suspend:
                    viewModel.StopReceiver();
                    break;
            }
        }

        private void Button1_MouseClick(object sender, MouseEventArgs e)
        {
            if (viewModel.IsRunning)
            {
                viewModel.StopReceiver();
            }
            else
            {
                viewModel.StartReceiver();
            }

            UpdateButtonText();
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            ShowMyself();
        }

        private void MainPage_Resize(object sender, System.EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            if (WindowState == FormWindowState.Minimized)
            {
                HideMyself();
            }
        }

        private void ShowMyself()
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void HideMyself()
        {
            Hide();
            notifyIcon1.Visible = true;
            UpdateNotifyIconText();
        }

        private void MainPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            SystemEvents.PowerModeChanged -= OnPowerChanged;
            ToastNotificationManagerCompat.Uninstall();
            viewModel.StopReceiver();
        }

        private void UpdateButtonText()
        {
            if (viewModel.IsRunning)
            {
                startStopButton.Text = "Stop";
            }
            else
            {
                startStopButton.Text = "Start";
            }
        }

        private void UpdateNotifyIconText()
        {
            notifyIcon1.Text = viewModel.IsRunning ? "Receiver - running" : "Receiver - stopped";
        }

        private void MainPage_Shown(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            Hide();
        }

        private void cancelShutdownButton_Click(object sender, EventArgs e)
        {
            viewModel.CancelShutdown();
        }
    }
}
