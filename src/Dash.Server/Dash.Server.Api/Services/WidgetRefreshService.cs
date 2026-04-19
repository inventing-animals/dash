using System.Text.Json;
using Dash.Server.Persistence;
using Dash.Server.WidgetHost;
using Dash.Server.Api.Hubs;
using Dash.WidgetSdk.Abstractions;
using Dash.WidgetSdk.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dash.Server.Api.Services;

public sealed class WidgetRefreshService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServerWidgetCatalog _catalog;
    private readonly IWidgetStatePublisher _publisher;
    private readonly IConnectedClientTracker _clientTracker;
    private readonly ILogger<WidgetRefreshService> _logger;

    private sealed class CacheEntry
    {
        public required Guid PageId { get; init; }
        public required WidgetInstanceConfiguration Config { get; init; }
        public required IServerWidget Widget { get; init; }
        public DateTimeOffset NextRunAt { get; set; } = DateTimeOffset.MinValue;
        public WidgetStateEnvelope? LastEnvelope { get; set; }
    }

    // Populated once on startup; never queried again unless reloaded.
    private Dictionary<string, CacheEntry>? _cache; // keyed by WidgetId.ToString()

    public WidgetRefreshService(
        IServiceScopeFactory scopeFactory,
        IServerWidgetCatalog catalog,
        IWidgetStatePublisher publisher,
        IConnectedClientTracker clientTracker,
        ILogger<WidgetRefreshService> logger)
    {
        _scopeFactory = scopeFactory;
        _catalog = catalog;
        _publisher = publisher;
        _clientTracker = clientTracker;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await LoadCacheAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await TickAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Unhandled error during widget refresh tick.");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }

    private async Task LoadCacheAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DashDbContext>();

        var allWidgets = await db.Widgets
            .AsNoTracking()
            .Select(w => new { w.PageId, w.WidgetId, w.WidgetType, w.ConfigurationJson, w.StateJson })
            .ToListAsync(ct);

        var cache = new Dictionary<string, CacheEntry>(allWidgets.Count);

        foreach (var row in allWidgets)
        {
            if (!_catalog.TryGet(row.WidgetType, out var widget) || widget is null)
                continue;

            WidgetInstanceConfiguration config;
            try
            {
                var configElement = JsonSerializer.Deserialize<JsonElement>(row.ConfigurationJson);
                config = new WidgetInstanceConfiguration(row.WidgetId.ToString(), row.WidgetType, configElement);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse configuration for widget {WidgetId}.", row.WidgetId);
                continue;
            }

            WidgetStateEnvelope? lastEnvelope = null;
            if (row.StateJson is not null)
            {
                try
                {
                    lastEnvelope = JsonSerializer.Deserialize<WidgetStateEnvelope>(row.StateJson);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to restore cached state for widget {WidgetId}.", row.WidgetId);
                }
            }

            cache[row.WidgetId.ToString()] = new CacheEntry
            {
                PageId = row.PageId,
                Config = config,
                Widget = widget,
                LastEnvelope = lastEnvelope,
            };
        }

        _cache = cache;
        _logger.LogInformation("Widget cache loaded: {Count} widget(s).", cache.Count);
    }

    private async Task TickAsync(CancellationToken ct)
    {
        if (!_clientTracker.HasConnectedClients)
            return;

        var cache = _cache;
        if (cache is null)
            return;

        var now = DateTimeOffset.UtcNow;
        var resultsByPage = new Dictionary<Guid, List<WidgetStateEnvelope>>();

        foreach (var (instanceId, entry) in cache)
        {
            if (now < entry.NextRunAt)
                continue;

            WidgetStateEnvelope newEnvelope;
            try
            {
                newEnvelope = await entry.Widget.ExecuteAsync(
                    new ServerWidgetExecutionRequest(entry.Config), ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Widget {WidgetType} ({InstanceId}) threw during execution.",
                    entry.Config.WidgetType, instanceId);
                // Still advance NextRunAt so we don't hammer a broken widget every tick.
                entry.NextRunAt = now + entry.Widget.GetRefreshInterval(entry.Config);
                continue;
            }

            entry.NextRunAt = now + entry.Widget.GetRefreshInterval(entry.Config);

            // Skip publishing and persisting when the state hasn't actually changed.
            var newRaw = newEnvelope.State.GetRawText();
            var oldRaw = entry.LastEnvelope?.State.GetRawText();
            if (newRaw == oldRaw)
                continue;

            entry.LastEnvelope = newEnvelope;

            // Persist asynchronously; don't block the tick.
            _ = PersistStateAsync(instanceId, newEnvelope);

            if (!resultsByPage.TryGetValue(entry.PageId, out var list))
                resultsByPage[entry.PageId] = list = [];

            list.Add(newEnvelope);
        }

        foreach (var (pageId, envelopes) in resultsByPage)
        {
            await _publisher.PublishToPageAsync(pageId, envelopes, ct);
        }
    }

    private async Task PersistStateAsync(string instanceId, WidgetStateEnvelope envelope)
    {
        try
        {
            var widgetId = Guid.Parse(instanceId);
            var stateJson = JsonSerializer.Serialize(envelope);

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DashDbContext>();

            await db.Widgets
                .Where(w => w.WidgetId == widgetId)
                .ExecuteUpdateAsync(s => s.SetProperty(w => w.StateJson, stateJson));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to persist state for widget {InstanceId}.", instanceId);
        }
    }
}
