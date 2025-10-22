using UnityEngine;

public class SMG : Weapon
{
    public override void Update()
    {
        base.Update();

        if (Input.GetMouseButton(0))
        {
            PrimaryAction();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SecondaryAction();
        }
    }

    public override void PrimaryAction()
    {
        if (TryShoot())
        {
            Vector2 target = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            Vector2 myPos = new(transform.position.x, transform.position.y + 1);
            Vector2 direction = target - myPos;
            direction.Normalize();

            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f);

            GameObject boolet;
            float spreadAngle;
            Vector2 spreadDirection;
            float perProjectileDamage = CalculateDamage();

            for (int i = 0; i < gunStats.projectilesPerAttack; i++)
            {
                boolet = ObjectPooling.Instance.GetPooledProjectile();
                if (boolet == null)
                    return;
                boolet.transform.SetPositionAndRotation(myPos, rotation);
                boolet.SetActive(true);
                spreadAngle = Random.Range(-gunStats.spread / 2, gunStats.spread / 2);
                spreadDirection = Quaternion.Euler(0, 0, spreadAngle) * direction;

                if (boolet.TryGetComponent<BooletController>(out var booletC))
                {
                    booletC.Damage = perProjectileDamage;
                    booletC.Rb2d.AddForce(gunStats.projectileSpeed * spreadDirection);
                    booletC.maxLifespan = gunStats.projectileLifespan * (1 + ownerCreatureSO.GetStat(StatInfo.Stat.PercentBonusProjectileLifespan) / 100);
                    booletC.Activate();
                }
            }
        }
    }

    public override void SecondaryAction()
    {
        TryReload();
    }

    public override void TertiaryAction()
    {
        // Implement any tertiary action for the pistol here
    }

    public override void HandleShoot()
    {
        CurrentAmmo--;
    }

    public override void ResetGraphics()
    {
        //transform.GetComponent<SpriteRenderer>().color = Color.white; // Revert color after reloading
    }
}