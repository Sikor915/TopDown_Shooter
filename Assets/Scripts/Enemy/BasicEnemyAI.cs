using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class BasicEnemyAI : MonoBehaviour
{   
    public Patrol patrolState = new();
    public Follow followState = new();
    public Attack attackState = new();

    AiState currentState;
    public AiState CurrentState { get { return currentState; } }

    EnemyController enemyController;
    public EnemyController EnemyController { get { return enemyController; } }

    List<Vector2Int> patrolPoints = new();

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        currentState?.OnStateUpdate();
    }

    public void ActivateEnemy()
    {
        ChangeState(patrolState);
    }

    public void DeactivateEnemy()
    {
        ChangeState(null);
    }

    public void ChangeState(AiState newState)
    {
        currentState?.OnStateExit();

        Debug.Log("Changing state to: " + (newState != null ? newState.GetType().Name : "null"));

        currentState = newState;
        currentState?.OnStateEnter(this);
    }

    public void SetPatrolPoints(List<Vector2Int> points)
    {
        patrolPoints = points;
    }

    public Vector2Int GetRandomPatrolPoint()
    {
        if (patrolPoints.Count == 0)
        {
            Debug.LogWarning("No patrol points set for BasicEnemyAI!");
            return new Vector2Int((int)transform.position.x, (int)transform.position.y);
        }
        return patrolPoints[Random.Range(0, patrolPoints.Count)];
    }
}
