using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Dash.Client.Api;
using Dash.WidgetSdk.Abstractions;

namespace Dash.Client.Core;

public sealed record State(
    Mode Mode,
    Guid? CurrentPageId,
    ImmutableDictionary<string, WidgetStateEnvelope> WidgetStates,
    IReadOnlyList<PageData> Pages
)
{
    public static State Initial => new(
        Mode.Dashboard,
        null,
        ImmutableDictionary<string, WidgetStateEnvelope>.Empty,
        []
    );
}