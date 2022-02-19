using System;
using System.Text;

namespace ReceiverWinFormsApp.Builders
{
    public class ShutdownCommandArgumentsBuilder
    {
        int seconds;
        bool overrideExistingShutdown;
        bool showNotification;

        public ShutdownCommandArgumentsBuilder Seconds(int seconds)
        {
            if (seconds < 0)
                throw new ArgumentException(
                    $"seconds expected to be a positive integer, but was {seconds}");
            this.seconds = seconds;
            return this;
        }

        public ShutdownCommandArgumentsBuilder OverrideExistingShutdown(
            bool overrideExistingShutdown)
        {
            this.overrideExistingShutdown = overrideExistingShutdown;
            return this;
        }

        public ShutdownCommandArgumentsBuilder ShowNotification(bool showNotification)
        {
            this.showNotification = showNotification;
            return this;
        }

        public string Build()
        {
            var result = new StringBuilder();
            result.Append(overrideExistingShutdown ?
                $"/C SHUTDOWN /A & SHUTDOWN /S /T {seconds}" :
                $"/C SHUTDOWN /S /T {seconds}");
            if (!showNotification)
                result.Append(" /c \" \"");
            return result.ToString();
        }
    }
}
