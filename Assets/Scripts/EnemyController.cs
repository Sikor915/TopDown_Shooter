using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour, IEnemy {

    [SerializeField] int maxHealth;
    [SerializeField] int damage;
    [SerializeField] int enemyScore;
    [SerializeField] PlayerSO playerSO;

    public static event Action<int> OnEnemyKilled;

    Rigidbody2D rb2d;
    int health;
    bool isColliding = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        health = maxHealth;
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        if (isColliding)
        {
            Debug.Log("Colliding with player");
            if (playerSO.CanBeHit)
            {
                playerSO.OnHit(damage);
            }
            else
            {
                Debug.Log("Player is invulnerable");
            }
        }
    }

    public bool DeductHealth(int damage) {
        try {
            health -= damage;
            return true;
        } catch (Exception) {
            return false;
        }
    }
    private void OnCollisionEnter2D(UnityEngine.Collision2D collision) {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit a player");
            playerSO.OnHit(damage);
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

    void OnDestroy() {
        OnEnemyKilled?.Invoke(enemyScore);
    }
}
