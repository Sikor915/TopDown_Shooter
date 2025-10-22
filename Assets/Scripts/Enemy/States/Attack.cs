using UnityEngine;

public class Attack : AiState
{
    protected override void OnEnter()
    {
        Debug.Log("Entering Attack State");
    }
    
    protected override void OnUpdate()
    {
        Debug.Log("Updating Attack State");
    }

    protected override void OnExit()
    {
        Debug.Log("Exiting Attack State");
    }

    protected override void OnHurt()
    {
        Debug.Log("Attack State: Hurt");
    }
}