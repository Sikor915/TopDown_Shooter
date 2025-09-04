using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class Weapon : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onShootEvent;
    public UnityEvent onReloadEvent;

    [Header("References")]
    public GunSO gunStats;
    public PlayerController pc;
    public GameObject projectilePrefab;

    int currentAmmo;
    public int CurrentAmmo
    {
        get { return currentAmmo; }
        set { currentAmmo = value; }
    }
    float nextAttackTime;
    public float NextAttackTime
    {
        get { return nextAttackTime; }
    }

    bool isReloading;
    public bool IsReloading
    {
        get { return isReloading; }
    }

    void Start()
    {
        onShootEvent ??= new UnityEvent();
        currentAmmo = gunStats.magazineSize;
        pc = transform.root.GetComponent<PlayerController>();
    }

    public void TryReload()
    {
        if (isReloading || gunStats.ammoReserve <= 0 || currentAmmo == gunStats.magazineSize) return;
        StartCoroutine(ReloadCoroutine());
    }

    public bool TryShoot()
    {
        if (isReloading || Time.time < nextAttackTime || currentAmmo <= 0) return false;

        // Shoot
        nextAttackTime = Time.time + 1f / gunStats.fireRate;
        onShootEvent.Invoke();
        return true;
    }

    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(gunStats.reloadTime);
        int ammoNeeded = gunStats.magazineSize - currentAmmo;

        if (gunStats.ammoReserve >= ammoNeeded)
        {
            currentAmmo += ammoNeeded;
            gunStats.ammoReserve -= ammoNeeded;
        }
        else
        {
            currentAmmo += gunStats.ammoReserve;
            gunStats.ammoReserve = 0;
        }
        onReloadEvent.Invoke();
        Debug.Log("Reloaded. Current ammo: " + currentAmmo + ", Ammo reserve: " + gunStats.ammoReserve);
        isReloading = false;
    }

    public abstract void PrimaryAction();
    public abstract void SecondaryAction();
    public abstract void TertiaryAction();
    public abstract void HandleShoot();
}
