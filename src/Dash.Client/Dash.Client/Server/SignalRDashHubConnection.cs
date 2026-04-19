using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dash.WidgetSdk.Abstractions;
using Microsoft.AspNetCore.SignalR.Client;

namespace Dash.Client.Server;

public sealed class SignalRDashHubConnection : IDashHubConnection
{
    private readonly HubConnection _connection;

    public event Action<IReadOnlyList<WidgetStateEnvelope>>? WidgetStatesReceived;

    public SignalRDashHubConnection(IServerConnectionSettingsService settings)
    {
        var hubUrl = new Uri(new Uri(settings.Get().RemoteHostUrl), "/hubs/dash");

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.On<IReadOnlyList<WidgetStateEnvelope>>(
            nameof(IDashClient.OnWidgetStatesUpdated),
            states => WidgetStatesReceived?.Invoke(states));
    }

    public Task StartAsync(CancellationToken ct = default)
        => _connection.StartAsync(ct);

    public Task SubscribeToPageAsync(Guid pageId, CancellationToken ct = default)
        => _connection.InvokeAsync("SubscribeToPageAsync", pageId, ct);

    public Task UnsubscribeFromPageAsync(Guid pageId, CancellationToken ct = default)
        => _connection.InvokeAsync("UnsubscribeFromPageAsync", pageId, ct);

    public ValueTask DisposeAsync()
        => _connection.DisposeAsync();
}
