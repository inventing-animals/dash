using System.Linq;

namespace Dash.Client.Core;

public static class Reducer
{
    public static State Reduce(State state, ClientAction action)
    {
        return action switch
        {
            Navigate a => state with { Mode = a.Mode },

            SelectPage a => state with { CurrentPageId = a.PageId },

            PagesLoaded a => state with
            {
                Pages = a.Pages,
                CurrentPageId = state.CurrentPageId ?? a.Pages.FirstOrDefault()?.PageId,
            },

            WidgetStatesReceived a => state with { WidgetStates = ApplyWidgetStates(state, a) },

            _ => state
        };
    }

    private static System.Collections.Immutable.ImmutableDictionary<string, Dash.WidgetSdk.Abstractions.WidgetStateEnvelope>
        ApplyWidgetStates(State state, WidgetStatesReceived action)
    {
        var builder = state.WidgetStates.ToBuilder();
        foreach (var envelope in action.States)
        {
            if (!state.WidgetStates.TryGetValue(envelope.InstanceId, out var existing)
                || envelope.UpdatedAtUtc > existing.UpdatedAtUtc)
            {
                builder[envelope.InstanceId] = envelope;
            }
        }
        return builder.ToImmutable();
    }
}