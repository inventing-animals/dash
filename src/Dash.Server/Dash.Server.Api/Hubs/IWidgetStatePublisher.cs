using Dash.WidgetSdk.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace Dash.Server.Api.Hubs;

public interface IWidgetStatePublisher
{
    Task PublishToPageAsync(Guid pageId, IReadOnlyList<WidgetStateEnvelope> states, CancellationToken ct = default);
}

public sealed class HubWidgetStatePublisher(IHubContext<DashHub, IDashClient> hub) : IWidgetStatePublisher
{
    public Task PublishToPageAsync(Guid pageId, IReadOnlyList<WidgetStateEnvelope> states, CancellationToken ct)
        => hub.Clients.Group(DashHub.PageGroup(pageId)).OnWidgetStatesUpdated(states);
}
