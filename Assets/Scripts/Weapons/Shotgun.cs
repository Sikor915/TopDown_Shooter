using System.Collections;
using UnityEngine;

public class Shotgun : Weapon
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
        if (IsReloading)
        {
            IsReloading = false; // Interrupt reloading for shotgun
            StopAllCoroutines();
            UIController.Instance.StopReloadProgressBar();
            transform.GetComponent<SpriteRenderer>().color = Color.white; // Revert color after interrupting
        }

        if (TryShoot())
        {
            Vector2 target = isUsedByPlayer ?
                (Vector2)Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)) :
                GameMaster.Instance.PlayerController.transform.position;
            Vector2 myPos = transform.GetChild(0).position;
            Vector2 direction = target - myPos;
            direction.Normalize();

            GameObject boolet;
            Quaternion rotation;
            float spreadAngle;
            Vector2 spreadDirection;
            float perProjectileDamage = CalculateDamage();

            for (int i = 0; i < gunStats.projectilesPerAttack; i++)
            {
                boolet = ObjectPooling.Instance.GetPooledProjectile();
                if (boolet == null)
                    return;

                rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f);
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

    protected override IEnumerator ReloadCoroutine()
    {
        IsReloading = true;
        yield return new WaitForSeconds(gunStats.reloadTime);
        int ammoNeeded = 1;
        if (gunStats.ammoReserve >= ammoNeeded)
        {
            CurrentAmmo += ammoNeeded;
            gunStats.ammoReserve -= ammoNeeded;
        }
        else
        {
            CurrentAmmo += gunStats.ammoReserve;
            gunStats.ammoReserve = 0;
        }
        onReloadEvent.Invoke(CurrentAmmo, gunStats.magazineSize, gunStats.ammoReserve);
        IsReloading = false;
        TryReload(); // Automatically reload the next shell if available
    }
}