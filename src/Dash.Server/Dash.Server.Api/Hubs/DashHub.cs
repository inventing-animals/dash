using Dash.WidgetSdk.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace Dash.Server.Api.Hubs;

public sealed class DashHub(IConnectedClientTracker tracker) : Hub<IDashClient>
{
    public override Task OnConnectedAsync()
    {
        tracker.OnConnected(Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        tracker.OnDisconnected(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SubscribeToPageAsync(Guid pageId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, PageGroup(pageId));
    }

    public async Task UnsubscribeFromPageAsync(Guid pageId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, PageGroup(pageId));
    }

    public static string PageGroup(Guid pageId) => $"page:{pageId}";
}
