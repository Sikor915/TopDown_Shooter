using UnityEngine;

public class Katana : Weapon
{
    Collider2D coll2d;
    [SerializeField] GameObject slashEffect;
    new void Awake()
    {
        usesAmmo = false;
        base.Awake();
        coll2d = transform.GetComponent<Collider2D>();
        coll2d.enabled = false; // Disable collider initially
        slashEffect.GetComponent<TrailRenderer>().enabled = false;
    }
    public override void Update()
    {
        base.Update();

        if (Input.GetMouseButton(0))
        {
            PrimaryAction();
        }
        if (Input.GetMouseButton(1))
        {
            SecondaryAction();
        }
    }

    public override void PrimaryAction()
    {
        if (TryShoot())
        {
            Debug.Log("Katana slash!");
        }
    }

    public override void SecondaryAction()
    {
        // Block/parry action can be implemented here
        Debug.Log("Katana block!");
    }

    public override void TertiaryAction()
    {
        // Implement any tertiary action for the pistol here
    }

    public override void HandleShoot()
    {
        // Implement slash
        Collider2D coll2d = transform.GetComponent<Collider2D>();
        coll2d.enabled = true; // Enable collider to detect hits
        slashEffect.GetComponent<TrailRenderer>().enabled = true;
        anim.SetTrigger("MeleeAttack");
        Invoke(nameof(DisableCollider), 0.4f); // Disable collider after short duration
    }

    public override void ResetGraphics()
    {
        transform.GetComponent<SpriteRenderer>().color = Color.white; // Revert color after reloading
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Katana collided with " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
            float damage = CalculateDamage();
            Debug.Log("Hit enemy for " + damage + " damage.");
            enemy.DeductHealth(damage);
        }
    }

    void DisableCollider()
    {
        coll2d.enabled = false;
        slashEffect.GetComponent<TrailRenderer>().enabled = false;
    }
}