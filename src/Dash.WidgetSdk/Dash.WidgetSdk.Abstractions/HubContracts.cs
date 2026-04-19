namespace Dash.WidgetSdk.Abstractions;

public interface IDashClient
{
    Task OnWidgetStatesUpdated(IReadOnlyList<WidgetStateEnvelope> states);
}
