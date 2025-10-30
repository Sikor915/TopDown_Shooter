using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BasicEnemyAI))]
public class EnemyController : MonoBehaviour, IEnemy
{
    [SerializeField] BasicEnemySO enemySO;
    [SerializeField] PlayerSO playerSO;
    [SerializeField] PlayerController playerController;
    [SerializeField] float fieldOfViewAngle = 60f;
    [SerializeField] float viewDistance = 10f;
    [SerializeField] float hearDistance = 5f;
    [SerializeField] float nearCheckDistance = 20f;


    public float HearDistance { get { return hearDistance; } }
    public static event Action<int> OnEnemyKilled;

    Rigidbody2D rb2d;
    float health;
    bool isColliding = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
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

        if (rb2d.linearVelocity.magnitude < 0.3f)
        {
            GetComponent<BasicEnemyAI>().patrolState.OnStuck();
        }
    }

    public void MoveTowards(Vector3 targetPosition)
    {
        rb2d.linearVelocity = (targetPosition - transform.position).normalized * enemySO.MoveSpeed;
        float angle = Mathf.Atan2(rb2d.linearVelocity.y, rb2d.linearVelocity.x) * Mathf.Rad2Deg;
        rb2d.rotation = angle;
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

    public bool ShootRaycastsInFront(Vector3 direction)
    {
        Debug.Log("Shooting raycasts in front");
        Vector3 origin = transform.position;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -fieldOfViewAngle / 2) * direction;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, fieldOfViewAngle / 2) * direction;
        //RaycastHit2D hitInfo;
        LayerMask playerLayer = LayerMask.GetMask("Player");

        int numberOfRays = 10;
        for (int i = 0; i < numberOfRays; i++)
        {
            float t = (float)i / (numberOfRays - 1);
            Vector3 rayDirection = Vector3.Slerp(leftBoundary, rightBoundary, t).normalized;

            RaycastHit2D hit = Physics2D.Raycast(origin, rayDirection, viewDistance, playerLayer);
            Debug.DrawRay(origin, rayDirection * viewDistance, Color.red);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                //hitInfo = hit;
                return true;
            }
        }

        //hitInfo = default;
        return false;
    }

    public bool IsPlayerNearby()
    {
        Vector3 directionToPlayer = playerController.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        if (distanceToPlayer <= nearCheckDistance)
        {
            return true;
        }
        return false;
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
