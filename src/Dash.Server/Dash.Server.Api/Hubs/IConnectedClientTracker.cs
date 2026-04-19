using System.Collections.Concurrent;

namespace Dash.Server.Api.Hubs;

public interface IConnectedClientTracker
{
    void OnConnected(string connectionId);
    void OnDisconnected(string connectionId);
    bool HasConnectedClients { get; }
}

public sealed class ConnectedClientTracker : IConnectedClientTracker
{
    private readonly ConcurrentDictionary<string, bool> _connections = new();

    public void OnConnected(string connectionId) => _connections[connectionId] = true;

    public void OnDisconnected(string connectionId) => _connections.TryRemove(connectionId, out _);

    public bool HasConnectedClients => !_connections.IsEmpty;
}
