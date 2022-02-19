using Domain;
using Microsoft.Toolkit.Uwp.Notifications;

namespace ReceiverWinFormsApp.Notification
{
    public class WindowsNotifier : INotifier
    {
        public void Notify(string message)
        {
            new ToastContentBuilder()
                .AddText(message)
                .Show();
        }
    }
}
