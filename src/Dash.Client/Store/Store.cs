using System;

namespace Dash.Client.Core;

public sealed class Store
{
    private readonly object _gate = new();
    private State _state;

    public Store(State? initialState = null)
    {
        _state = initialState ?? State.Initial;
    }

    public State State
    {
        get
        {
            lock (_gate)
            {
                return _state;
            }
        }
    }

    /// <summary>
    /// Raised after state changes.
    /// </summary>
    public event EventHandler<State>? StateChanged;

    public void Dispatch(ClientAction action)
    {
        Console.WriteLine($"Dispatching action: {action}");

        if (action is null) throw new ArgumentNullException(nameof(action));

        State next;
        lock (_gate)
        {
            var current = _state;
            next = Reducer.Reduce(current, action);

            if (ReferenceEquals(current, next) || current == next)
                return;

            _state = next;
        }

        StateChanged?.Invoke(this, next);
    }
}