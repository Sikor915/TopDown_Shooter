using UnityEngine;

public class Follow : AiState
{
    PlayerController player;
    protected override void OnEnter()
    {
        player = GameMaster.Instance.PlayerController;
    }
    
    // TODO: Implement following algorithm
    protected override void OnUpdate()
    {
        if (player != null)
        {
            if(controller.EnemyController.CanHitPlayer())
            {
                controller.ChangeState(controller.attackState);
                return;
            }

            Vector3 playerPosition = player.transform.position;
            controller.EnemyController.MoveTowards(playerPosition);

            if (!controller.EnemyController.IsPlayerNearby())
            {
                controller.ChangeState(controller.patrolState);
                return;
            }
        }
    }

    protected override void OnExit()
    {
        Debug.Log("Exiting Follow State");
        player = null;
    }

    protected override void OnHurt()
    {
        Debug.Log("Follow State: Hurt");
    }
}