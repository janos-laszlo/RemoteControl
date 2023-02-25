using Domain;
using Microsoft.Toolkit.Uwp.Notifications;
using System;

namespace ReceiverWinFormsApp.Notification
{
    public class WindowsNotifier : INotifier
    {
        public WindowsNotifier()
        {
            ToastNotificationManagerCompat.OnActivated += _ => OnActivated?.Invoke();
        }

        public event Action OnActivated;

        public void Notify(string message)
        {
            new ToastContentBuilder()
                .AddText(message)
                .Show();
        }
    }
}
