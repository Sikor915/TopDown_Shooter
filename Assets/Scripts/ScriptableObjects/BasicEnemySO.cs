using UnityEngine;

[CreateAssetMenu(fileName = "BasicEnemySO", menuName = "Scriptable Objects/BasicEnemySO")]
public class BasicEnemySO : CreatureSO
{
    [Header("Stats")]
    [SerializeField] float damage;
    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    [SerializeField] float attackRange;
    public float AttackRange
    {
        get { return attackRange; }
        set { attackRange = value; }
    }

    [SerializeField] int enemyScore;
    public int EnemyScore
    {
        get { return enemyScore; }
        set { enemyScore = value; }
    }
    [SerializeField] float moveSpeed;
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    [SerializeField] float patrolSpeed;
    public float PatrolSpeed
    {
        get { return patrolSpeed; }
        set { patrolSpeed = value; }
    }
}
