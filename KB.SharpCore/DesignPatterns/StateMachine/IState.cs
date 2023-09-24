namespace KB.SharpCore.DesignPatterns.StateMachine;

public interface IState<in TStateMachine>
{
    void OnStateEnter(TStateMachine stateMachine);
    IState<TStateMachine> OnUpdate(TStateMachine stateMachine, double delta);
    void OnStateLeave(TStateMachine stateMachine);

    bool CanEnter(TStateMachine stateMachine);
}