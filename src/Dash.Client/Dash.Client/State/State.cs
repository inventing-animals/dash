using System;

namespace Dash.Client.Core;

public sealed record State(
    Mode Mode,
    Guid? CurrentPageId
)

{
    public static State Initial => new(
        Mode.Dashboard,
        null
    );
}