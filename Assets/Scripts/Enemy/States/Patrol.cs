using UnityEngine;

public class Patrol : AiState
{
    Room currentRoom;
    Vector2Int pointToGo;
    protected override void OnEnter()
    {
        Debug.Log("Entering Patrol State");
        currentRoom = MapGenerator.Instance.GetCurrentRoom(new Vector2Int((int)controller.transform.position.x, (int)controller.transform.position.y));
        if (currentRoom == null)
        {
            Debug.LogWarning("Patrol State: Current room is null!");
            return;
        }
        controller.SetPatrolPoints(currentRoom.GetRandomPatrolPoints());
        pointToGo = controller.GetRandomPatrolPoint();
    }
    
    protected override void OnUpdate()
    {
        if (controller.EnemyController.IsPlayerNearby())
        {
            if (controller.EnemyController.ShootRaycastsInFront(controller.transform.right))
            {
                controller.ChangeState(controller.followState);
                return;
            }
        }
        if (pointToGo != null)
        {
            Vector3 targetPosition = new Vector3(pointToGo.x, pointToGo.y, 0);
            controller.EnemyController.PatrolTowards(targetPosition);

            if (Vector3.Distance(controller.transform.position, targetPosition) < 0.1f)
            {
                pointToGo = controller.GetRandomPatrolPoint();
            }
        }
    }

    protected override void OnExit()
    {
        Debug.Log("Exiting Patrol State");
        currentRoom = null;
    }

    protected override void OnHurt()
    {
        Debug.Log("Patrol State: Hurt");
    }

    public override void OnStuck()
    {
        Debug.Log("Patrol State: Stuck, choosing new patrol point");
        pointToGo = controller.GetRandomPatrolPoint();
    }
}