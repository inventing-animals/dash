using Dash.WidgetSdk.Serialization.Json;
using Dash.WidgetSdk.Server;

namespace Dash.Widgets.EmailChecker.Server;

public sealed class EmailCheckerServerWidget : IServerWidget
{
    public Dash.WidgetSdk.Abstractions.WidgetDefinition Definition => EmailCheckerWidget.Definition;

    public ValueTask<Dash.WidgetSdk.Abstractions.WidgetStateEnvelope> ExecuteAsync(
        ServerWidgetExecutionRequest request,
        CancellationToken cancellationToken)
    {
        var configuration = WidgetJsonSerializer.Deserialize<EmailCheckerWidgetConfiguration>(request.Instance.Configuration) ??
                            EmailCheckerWidgetConfiguration.Default;
        var utcNow = DateTimeOffset.UtcNow;

        // This is a deterministic placeholder until the real email integration exists.
        var unreadCount = Math.Abs(HashCode.Combine(
            request.Instance.InstanceId,
            configuration.AccountLabel,
            configuration.Folder,
            utcNow.Date,
            utcNow.Hour)) % 24;

        var state = new EmailCheckerWidgetState(
            unreadCount,
            utcNow,
            unreadCount == 0 ? "All caught up" : "Unread messages available");

        return ValueTask.FromResult(
            WidgetJsonSerializer.CreateStateEnvelope(
                request.Instance.InstanceId,
                request.Instance.WidgetType,
                state));
    }
}
