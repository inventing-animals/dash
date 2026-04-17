namespace Dash.Server.Api.Contracts.Users;

public sealed record GetUserPagesResponse(
    Guid UserId,
    string UserName,
    IReadOnlyList<PageSummaryResponse> Pages);

public sealed record PageSummaryResponse(
    Guid PageId,
    string Name,
    IReadOnlyList<WidgetSummaryResponse> Widgets);

public sealed record WidgetSummaryResponse(
    Guid WidgetId,
    string WidgetType,
    string ConfigurationJson);
