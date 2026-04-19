using System.Collections.Generic;
using Dash.WidgetSdk.Abstractions;

namespace Dash.Client.Core;

public sealed record WidgetStatesReceived(IReadOnlyList<WidgetStateEnvelope> States) : ClientAction;
