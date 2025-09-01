

public class StateMachine
{
    private IState currentState;

    public void ChangeState(IState st)
    {
        currentState?.Exit();   
        currentState = st;
        currentState?.Enter();
    }
    public void Update()
    {
        currentState?.Update();
    }
}
