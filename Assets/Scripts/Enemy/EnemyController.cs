using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour, IEnemy
{
    [SerializeField] BasicEnemySO enemySO;
    [SerializeField] PlayerSO playerSO;

    public static event Action<int> OnEnemyKilled;

    Rigidbody2D rb2d;
    float health;
    bool isColliding = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        health = enemySO.MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (isColliding)
        {
            if (playerSO.CanBeHit)
            {
                float totalDamage = CalculateDamage();
                playerSO.OnHit(totalDamage);
            }
        }
    }

    public void MoveTowards(Vector3 targetPosition)
    {
        rb2d.linearVelocity = (targetPosition - transform.position).normalized * enemySO.MoveSpeed;
    }

    public void Reset()
    {
        health = enemySO.MaxHealth;
        isColliding = false;
    }

    public bool DeductHealth(float damage)
    {
        try
        {
            health -= damage;
            enemySO.DeductHealth((int)damage);
            if (health <= 0)
            {
                Died();
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerSO.OnHit(CalculateDamage());
            isColliding = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            isColliding = false;
        }
    }

    void Died()
    {
        EnemySpawner.Instance.RemoveEnemy(gameObject);
        OnEnemyKilled?.Invoke(enemySO.EnemyScore);
        Debug.Log(EnemySpawner.Instance.Enemies.Count);
    }

    float CalculateDamage()
    {
        float totalFlatDamage = enemySO.Damage + enemySO.GetStat(StatInfo.Stat.BonusDamage);
        float totalDamage = totalFlatDamage * (1 + enemySO.GetStat(StatInfo.Stat.PercentBonusDamage));
        bool isCrit = Random.value < enemySO.GetStat(StatInfo.Stat.PercentCritChance);
        if (isCrit)
        {
            totalDamage *= (1 + enemySO.GetStat(StatInfo.Stat.PercentCritDamage));
        }
        return totalDamage;
    }
}
