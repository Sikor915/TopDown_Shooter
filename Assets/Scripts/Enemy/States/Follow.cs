using System.Collections.Generic;
using UnityEngine;

public class Follow : AiState
{
    PlayerController player;
    List<Vector3> path;
    int index = 0;
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
            //Doesn't work
            path = PathfinderManager.Instance.FindPath(
                controller.EnemyController.transform.position,
                player.transform.position
            );
            index = 0;
            if (path != null && path.Count > 0)
            {
                Vector3 targetPos = path[index];
                controller.EnemyController.MoveTowards(targetPos);

                if (Vector3.Distance(controller.EnemyController.transform.position, targetPos) < 0.1f)
                {
                    index++;
                    if (index >= path.Count)
                    {
                        index = path.Count - 1;
                    }
                }
            }

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