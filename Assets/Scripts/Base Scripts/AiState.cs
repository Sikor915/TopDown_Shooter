using System.Diagnostics;

public abstract class AiState
{

    public BasicEnemyAI controller;

    public void OnStateEnter(BasicEnemyAI controller)
    {
        this.controller = controller;
        OnEnter();
    }

    protected virtual void OnEnter()
    {

    }
    
    public void OnStateExit()
    {
        OnExit();
    }
    
    protected virtual void OnExit()
    {
        
    }

    public void OnStateUpdate()
    {
        OnUpdate();
    }
    
    protected virtual void OnUpdate()
    {
        
    }

    public void OnStateHurt()
    {
        OnHurt();
    }

    protected virtual void OnHurt()
    {
        
    }
}
