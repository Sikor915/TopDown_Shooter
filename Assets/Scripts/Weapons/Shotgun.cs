using System.Collections;
using UnityEngine;

public class Shotgun : Weapon
{
    IEnumerator reloadCoroutine;
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
        if (IsReloading)
        {
            IsReloading = false; // Interrupt reloading for shotgun
            StopAllCoroutines();
            transform.GetComponent<SpriteRenderer>().color = Color.white; // Revert color after interrupting
        }

        if (TryShoot())
        {
            Vector2 target = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            Vector2 myPos = new(transform.position.x, transform.position.y + 1);
            Vector2 direction = target - myPos;
            direction.Normalize();
            GameObject boolet;
            Quaternion rotation;
            float spreadAngle;
            Vector2 spreadDirection;
            for (int i = 0; i < gunStats.projectilesPerAttack; i++)
            {
                rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f);
                boolet = Instantiate(projectilePrefab, myPos, rotation);
                spreadAngle = Random.Range(-gunStats.spread / 2, gunStats.spread / 2);
                spreadDirection = Quaternion.Euler(0, 0, spreadAngle) * direction;
                if (boolet.TryGetComponent<BooletController>(out var booletC))
                {
                    booletC.Rb2d.AddForce(spreadDirection * booletC.BulletSpeed);
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

    protected override IEnumerator ReloadCoroutine()
    {
        IsReloading = true;
        transform.GetComponent<SpriteRenderer>().color = Color.green; // Change color to indicate reloading
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
        onReloadEvent.Invoke();
        transform.GetComponent<SpriteRenderer>().color = Color.white; // Revert color after reloading
        IsReloading = false;
        TryReload(); // Automatically reload the next shell if available
    }

    public override void ResetGraphics()
    {
        //transform.GetComponent<SpriteRenderer>().color = Color.white; // Revert color after reloading
    }
}