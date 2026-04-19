using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dash.Client.Api;

public interface IDashApiClient
{
    Task<IReadOnlyList<PageData>> GetPagesAsync(CancellationToken ct = default);
}
