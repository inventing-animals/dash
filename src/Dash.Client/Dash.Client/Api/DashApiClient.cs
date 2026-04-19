using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Dash.Client.Server;

namespace Dash.Client.Api;

public sealed class DashApiClient : IDashApiClient, IDisposable
{
    // TODO: replace with real user identity once auth is in place.
    private static readonly Guid DevUserId = Guid.Parse("6a827d52-d15a-4944-9f71-0d925e538001");

    private readonly HttpClient _http;

    public DashApiClient(IServerConnectionSettingsService settings)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(settings.Get().RemoteHostUrl),
        };
    }

    public async Task<IReadOnlyList<PageData>> GetPagesAsync(CancellationToken ct = default)
    {
        var response = await _http.GetFromJsonAsync<UserPagesResponse>(
            $"/api/users/{DevUserId}/pages", ct);

        return response?.Pages ?? [];
    }

    public void Dispose() => _http.Dispose();

    // Mirrors the server's GetUserPagesResponse shape.
    private sealed record UserPagesResponse(
        Guid UserId,
        string UserName,
        IReadOnlyList<PageData> Pages);
}
