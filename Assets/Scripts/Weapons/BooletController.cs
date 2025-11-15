using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BooletController : MonoBehaviour {
    [DefaultValue(2.0f)] public float maxLifespan;
    [SerializeField] float damage;
    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }
    Rigidbody2D rb2d;
    public Rigidbody2D Rb2d
    {
        get { return rb2d; }
    }
    private IEnemy enemy;

    void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void Activate()
    {
        StartCoroutine(DeactivateAfterTime());
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            enemy = collision.gameObject.GetComponent<IEnemy>();
            Debug.Log("Hit enemy for " + Damage + " damage.");
            if (enemy.DeductHealth(Damage))
            {
                StopAllCoroutines();
                ObjectPooling.Instance.ReturnProjectileToPool(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerController>().PlayerSO;
            Debug.Log("Hit player for " + Damage + " damage.");
            player.OnHit(Damage);
            StopAllCoroutines();
            ObjectPooling.Instance.ReturnProjectileToPool(gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            StopAllCoroutines();
            ObjectPooling.Instance.ReturnProjectileToPool(gameObject);
        }
    }

    void Deactivate()
    {
        ObjectPooling.Instance.ReturnProjectileToPool(gameObject);
    }

    System.Collections.IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(maxLifespan);
        Deactivate();
    }
}
