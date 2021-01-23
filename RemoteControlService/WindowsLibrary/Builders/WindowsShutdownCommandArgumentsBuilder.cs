using Domain.Builders;
using System;
using System.Text;

namespace WindowsLibrary.Builders
{
    public class WindowsShutdownCommandArgumentsBuilder : IShutdownCommandArgumentsBuilder
    {
        public int Seconds { get; private set; }
        public bool OverrideExistingShutdown { get; private set; }
        public bool ShowNotification { get; private set; }

        public IShutdownCommandArgumentsBuilder WithSeconds(int seconds)
        {
            if (seconds < 0)
                throw new ArgumentException(
                    $"seconds expected to be a positive integer, but was {seconds}");
            Seconds = seconds;
            return this;
        }

        public IShutdownCommandArgumentsBuilder ShouldOverrideExistingShutdown(
            bool overrideExistingShutdown)
        {
            OverrideExistingShutdown = overrideExistingShutdown;
            return this;
        }

        public IShutdownCommandArgumentsBuilder ShouldShowNotification(bool showNotification)
        {
            ShowNotification = showNotification;
            return this;
        }

        public string Build()
        {
            var result = new StringBuilder();
            result.Append(OverrideExistingShutdown ?
                $"/C SHUTDOWN /A & SHUTDOWN /S /T {Seconds}" :
                $"/C SHUTDOWN /S /T {Seconds}");
            if (!ShowNotification)
                result.Append(" /c \" \"");
            return result.ToString();
        }
    }
}
