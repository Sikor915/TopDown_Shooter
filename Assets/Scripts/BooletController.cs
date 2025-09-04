using System.ComponentModel;
using UnityEngine;

public class BooletController : MonoBehaviour {
    [SerializeField][DefaultValue(2.0f)] float maxLifespan;
    [SerializeField] int damage;
    [SerializeField] float bulletSpeed;

    Rigidbody2D rb2d;
    private IEnemy enemy;

    void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
        Destroy(this.gameObject, maxLifespan);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        Vector2 target = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = target - myPos;
        direction.Normalize();
        rb2d.AddForce(direction * bulletSpeed);
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            enemy = collision.gameObject.GetComponent<IEnemy>();
            if (enemy.DeductHealth(damage)) {
                Destroy(this.gameObject);
            }
        }
    }
}
