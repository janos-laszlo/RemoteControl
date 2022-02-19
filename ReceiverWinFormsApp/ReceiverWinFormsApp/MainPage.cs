using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace ReceiverWinFormsApp
{
    public partial class MainPage : Form
    {
        private readonly MainPageViewModel viewModel;

        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            SystemEvents.PowerModeChanged += OnPowerChanged;
            this.viewModel = viewModel;

            viewModel.StartReceiver();
            UpdateButtonText();
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
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void MainPage_Resize(object sender, System.EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
                UpdateNotifyIconText();
            }
        }

        private void MainPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            SystemEvents.PowerModeChanged -= OnPowerChanged;
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
    }
}
