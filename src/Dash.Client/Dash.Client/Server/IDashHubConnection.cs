using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dash.WidgetSdk.Abstractions;

namespace Dash.Client.Server;

public interface IDashHubConnection : IAsyncDisposable
{
    Task StartAsync(CancellationToken ct = default);
    Task SubscribeToPageAsync(Guid pageId, CancellationToken ct = default);
    Task UnsubscribeFromPageAsync(Guid pageId, CancellationToken ct = default);

    event Action<IReadOnlyList<WidgetStateEnvelope>> WidgetStatesReceived;
}
