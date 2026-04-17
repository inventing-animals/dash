using Dash.WidgetSdk.Abstractions;

namespace Dash.Widgets.EmailChecker;

public sealed record EmailCheckerWidgetConfiguration(
    string AccountLabel,
    string Folder,
    string PollingInterval)
{
    public static EmailCheckerWidgetConfiguration Default { get; } =
        new("Primary inbox", "Inbox", "00:05:00");
}

public sealed record EmailCheckerWidgetState(
    int UnreadCount,
    DateTimeOffset LastCheckedAtUtc,
    string Status);

public static class EmailCheckerWidget
{
    public const string Type = "dash.widgets.email-checker";

    public static WidgetDefinition Definition { get; } = new(
        Type,
        WidgetExecutionMode.ClientAndServer,
        new WidgetDisplayMetadata(
            "Email Checker",
            "Server-executed widget that publishes unread mail state for the client to render."),
        typeof(EmailCheckerWidgetConfiguration).FullName ?? nameof(EmailCheckerWidgetConfiguration),
        typeof(EmailCheckerWidgetState).FullName ?? nameof(EmailCheckerWidgetState));
}
