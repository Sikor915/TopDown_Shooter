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
        Invoke(nameof(Deactivate), maxLifespan);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            enemy = collision.gameObject.GetComponent<IEnemy>();
            Debug.Log("Hit enemy for " + Damage + " damage.");
            if (enemy.DeductHealth(Damage))
            {
                ObjectPooling.Instance.ReturnProjectileToPool(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Door"))
        {
            ObjectPooling.Instance.ReturnProjectileToPool(gameObject);
        }
    }

    void Deactivate()
    {
        ObjectPooling.Instance.ReturnProjectileToPool(gameObject);
    }
}
