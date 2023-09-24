namespace KB.SharpCore.DesignPatterns.StateMachine;

/// <summary>
///     Base state machine behaviour for changing states and executing states.
/// </summary>
/// <typeparam name="TStateMachine"></typeparam>
public abstract partial class BaseStateMachine<TStateMachine>
    where TStateMachine : BaseStateMachine<TStateMachine>
{
    protected readonly Dictionary<string, IState<TStateMachine>> m_statesDict = null;

    public IState<TStateMachine> CurrentState { get; private set; }

    // Update is called once per frame
    public virtual void Update(double delta)
    {
        IState<TStateMachine> newState = CurrentState.OnUpdate((TStateMachine)this, delta);
        if (newState != null)
            m_ChangeCurrentState(newState);
    }

    protected void m_ChangeCurrentState(IState<TStateMachine> newState)
    {
        if (newState == null || !newState.CanEnter((TStateMachine)this))
            return;

        CurrentState?.OnStateLeave((TStateMachine)this);
        CurrentState = newState;
        CurrentState.OnStateEnter((TStateMachine)this);
    }

    public IState<TStateMachine> GetStateInstance(string stateName)
    {
        return m_statesDict.TryGetValue(stateName, out IState<TStateMachine> state) ? state : null;
    }
}