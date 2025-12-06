using UnityEngine;

public class Carbine : Weapon
{
    ParticleSystem muzzleFlash;

    void Start()
    {
        muzzleFlash = transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
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
            Vector2 target = isUsedByPlayer ?
                (Vector2)Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)) :
                GameMaster.Instance.PlayerController.transform.position;
            Vector2 myPos = transform.GetChild(0).position;
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

    }

    public override void TertiaryAction()
    {
        // Implement any tertiary action for the pistol here
    }

    public override void ReloadAction()
    {
        TryReload();
    }

    public override void BotUse()
    {
        PrimaryAction();
    }

    public override void HandleShoot()
    {
        CurrentAmmo--;
        if( muzzleFlash.isPlaying )
        {
            muzzleFlash.Stop();
            muzzleFlash.Play();
        }
        else
        {
            muzzleFlash.Play();
        }
        AudioSource.PlayClipAtPoint(shootSound, transform.position);
    }
}