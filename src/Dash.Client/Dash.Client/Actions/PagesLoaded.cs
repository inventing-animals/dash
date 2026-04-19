using System.Collections.Generic;
using Dash.Client.Api;

namespace Dash.Client.Core;

public sealed record PagesLoaded(IReadOnlyList<PageData> Pages) : ClientAction;
