using UnityEngine;

public class Follow : AiState
{
    protected override void OnEnter()
    {
        Debug.Log("Entering Follow State");
    }
    
    protected override void OnUpdate()
    {
        Debug.Log("Updating Follow State");
    }

    protected override void OnExit()
    {
        Debug.Log("Exiting Follow State");
    }

    protected override void OnHurt()
    {
        Debug.Log("Follow State: Hurt");
    }
}