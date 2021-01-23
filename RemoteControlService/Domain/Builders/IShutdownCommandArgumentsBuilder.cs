namespace Domain.Builders
{
    public interface IShutdownCommandArgumentsBuilder
    {
        int Seconds { get; }
        bool OverrideExistingShutdown { get; }
        bool ShowNotification { get; }

        string Build();
        IShutdownCommandArgumentsBuilder ShouldOverrideExistingShutdown(bool overrideExistingShutdown);
        IShutdownCommandArgumentsBuilder ShouldShowNotification(bool showNotification);
        IShutdownCommandArgumentsBuilder WithSeconds(int seconds);
    }
}