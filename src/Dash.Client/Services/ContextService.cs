namespace Dash.Client.Core;

public class ContextService : IContextService
{
    private readonly Store _store = new();

    public Store getStore()
    {
        return _store;
    }
}