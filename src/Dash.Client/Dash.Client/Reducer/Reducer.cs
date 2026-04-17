namespace Dash.Client.Core;

public static class Reducer
{
    public static State Reduce(State state, ClientAction action)
    {
        return action switch
        {
            Navigate a => state with { Mode = a.Mode },

            SelectPage a => state with { CurrentPageId = a.PageId },

            _ => state
        };
    }
}