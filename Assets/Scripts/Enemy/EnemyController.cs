using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BasicEnemyAI))]
public class EnemyController : MonoBehaviour, IEnemy
{
    [Header("References")]
    [SerializeField] BasicEnemySO enemySO;
    [SerializeField] PlayerSO playerSO;
    [SerializeField] PlayerController playerController;
    [SerializeField] Weapon enemyWeapon;

    [Header("Enemy Senses")]
    [SerializeField] float fieldOfViewAngle;
    [SerializeField] float viewDistance;
    [SerializeField] float hearDistance;
    [SerializeField] float nearCheckDistance;


    public float HearDistance { get { return hearDistance; } }
    public static event Action<int> OnEnemyKilled;

    GameObject enemyWeaponGO;

    Rigidbody2D rb2d;
    public float health;
    bool isColliding = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GameMaster.Instance.PlayerController;
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
            GetComponent<BasicEnemyAI>().CurrentState.OnStuck();
        }
    }

    public void ActivateEnemy()
    {
        Reset();
        gameObject.SetActive(true);
        enemyWeapon.ownerCreatureSO = enemySO;
        enemyWeaponGO = Instantiate(enemyWeapon.gameObject, transform);
        enemyWeaponGO.SetActive(true);
        enemyWeaponGO.transform.localScale = new(0.5f, 0.5f, 1f);
        enemyWeaponGO.transform.localPosition = Vector3.zero + new Vector3(0.5f, 0f, 0f);
    }

    public void MoveTowards(Vector3 targetPosition, bool willAttack = false)
    {
        float speed = willAttack ? enemySO.MoveSpeed / 5 : enemySO.MoveSpeed;
        rb2d.linearVelocity = (targetPosition - transform.position).normalized * speed;
        float angle = Mathf.Atan2(rb2d.linearVelocity.y, rb2d.linearVelocity.x) * Mathf.Rad2Deg;
        rb2d.rotation = angle;
    }

    public void PatrolTowards(Vector3 targetPosition)
    {
        rb2d.linearVelocity = (targetPosition - transform.position).normalized * enemySO.PatrolSpeed;
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
            StartCoroutine(FlashRed());
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
        Vector3 origin = transform.position;
        Vector3 leftBoundary = Quaternion.Euler(0, 0, -fieldOfViewAngle / 2) * direction;
        Vector3 rightBoundary = Quaternion.Euler(0, 0, fieldOfViewAngle / 2) * direction;
        LayerMask mask = LayerMask.GetMask("Player", "Wall", "Objects");

        int numberOfRays = 10;
        for (int i = 0; i < numberOfRays; i++)
        {
            float t = (float)i / (numberOfRays - 1);
            Vector3 rayDirection = Vector3.Slerp(leftBoundary, rightBoundary, t).normalized;

            RaycastHit2D hit = Physics2D.Raycast(origin, rayDirection, viewDistance, mask);
            Debug.DrawRay(origin, rayDirection * viewDistance, Color.red);

            if (hit.collider != null)
            {
                int hitLayer = hit.collider.gameObject.layer;
                if (hitLayer == LayerMask.NameToLayer("Player"))
                    return true;
            }
        }

        return false;
    }

    public bool CanHitPlayer()
    {
        Vector3 directionToPlayer = playerController.transform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > viewDistance)
            return false;

        float angleToPlayer = Vector3.Angle(transform.right, directionToPlayer);
        if (angleToPlayer > fieldOfViewAngle / 2)
            return false;

        LayerMask mask = LayerMask.GetMask("Wall", "Objects");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer.normalized, enemySO.AttackRange, mask);
        Debug.DrawRay(transform.position, directionToPlayer.normalized * enemySO.AttackRange, Color.blue);

        if (hit.collider != null)
            return false;

        return true;
    }

    public void AttackPlayer()
    {
        if (enemyWeaponGO == null)
        {
            return;
        }
        if (enemyWeaponGO.GetComponent<Weapon>().CurrentAmmo <= 0 && enemyWeaponGO.GetComponent<Weapon>().usesAmmo)
        {
            enemyWeaponGO.GetComponent<Weapon>().TryReload();
            return;
        }
        Debug.Log("Enemy Attacking");
        enemyWeaponGO.GetComponent<Weapon>().PrimaryAction();
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
        int chance = Random.Range(0, 100);
        if (chance < 50)
        {
            Debug.Log("Dropping Money");
            GameObject moneyGO = Instantiate(MoneyManager.Instance.MoneyPrefab, transform.position, Quaternion.identity);
            MoneyObject moneyObject = moneyGO.GetComponent<MoneyObject>();
            moneyObject.Amount = enemySO.MoneyDropAmount;
        }
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

    IEnumerator FlashRed()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
}
