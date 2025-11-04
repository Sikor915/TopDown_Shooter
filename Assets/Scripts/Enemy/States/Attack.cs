using UnityEngine;

public class Attack : AiState
{
    PlayerController player;
    protected override void OnEnter()
    {
        player = GameMaster.Instance.PlayerController;
        Debug.Log("Entering Attack State");
    }
    
    protected override void OnUpdate()
    {
        if (player != null)
        {
            if (!controller.EnemyController.CanHitPlayer())
            {
                controller.ChangeState(controller.followState);
                return;
            }
            Vector3 shootingPosition = player.transform.position;
            controller.EnemyController.MoveTowards(shootingPosition, true);
            controller.EnemyController.AttackPlayer();
        }
    }

    protected override void OnExit()
    {
        player = null;
        Debug.Log("Exiting Attack State");
    }

    protected override void OnHurt()
    {
        Debug.Log("Attack State: Hurt");
    }
}