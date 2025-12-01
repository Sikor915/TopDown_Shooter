using UnityEngine;

public class Katana : Weapon
{
    Collider2D coll2d;
    [SerializeField] GameObject slashEffect;

    Vector3 basePosition = new Vector3(-0.65f, -0.11f, 0f);
    Quaternion baseRotation = Quaternion.Euler(0f, 0f, -125.3f);

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

        if (isUsedByPlayer)
        {
            if (Input.GetMouseButton(0))
            {
                PrimaryAction();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReloadAction();
            }
        }
        else
        {
            // AI usage can be handled differently if needed
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

    public override void ReloadAction()
    {
        // Katana does not reload
        Debug.Log("Katana does not require reloading.");
    }

    public override void BotUse()
    {
        PrimaryAction();
    }

    public override void HandleShoot()
    {
        // Implement slash
        Collider2D coll2d = transform.GetComponent<Collider2D>();
        coll2d.enabled = true; // Enable collider to detect hits
        slashEffect.GetComponent<TrailRenderer>().enabled = true;
        anim.SetTrigger("MeleeAttack");
        StartCoroutine(DisableColliderAfterDelay(0.3f)); // Disable collider after short delay
    }

    protected override void CorrectSpriteGraphics(PlayerAim.FacingDirection facingDir)
    {
        
    }

    public (Vector3, Quaternion) GetBaseTransform()
    {
        return (basePosition, baseRotation);
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
        if (IsBeingThrown)
        {
            if (TryGetComponent<Rigidbody2D>(out var rb2d))
            {
                Debug.Log("RB2D found on thrown katana.");
                rb2d.linearVelocity = Vector2.zero;
                rb2d.angularVelocity = 0;
                Destroy(rb2d);
                DisableCollider();
                DropWeapon(transform.position);
            }
        }
    }

    void DisableCollider()
    {
        coll2d.enabled = false;
        slashEffect.GetComponent<TrailRenderer>().enabled = false;
    }

    System.Collections.IEnumerator DisableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisableCollider();
    }
}