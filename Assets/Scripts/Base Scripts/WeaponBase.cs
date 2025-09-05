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
    [HideInInspector] public PlayerController pc;
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
        set { isReloading = value; }
    }

    protected void Awake()
    {
        onShootEvent ??= new UnityEvent();
        onReloadEvent ??= new UnityEvent();
        currentAmmo = gunStats.magazineSize;
        pc = transform.root.GetComponent<PlayerController>();
        if (projectilePrefab.TryGetComponent<BooletController>(out var booletC))
        {
            booletC.Damage = gunStats.damage;
        }
    }

    public virtual void Update()
    {
        // This can be overridden in derived classes if needed
    }

    virtual public void TryReload()
    {
        if (isReloading || gunStats.ammoReserve <= 0 || currentAmmo == gunStats.magazineSize) return;
        StartCoroutine(ReloadCoroutine());
    }

    public virtual bool TryShoot()
    {
        if (isReloading || Time.time < nextAttackTime || currentAmmo <= 0) return false;

        // Shoot
        nextAttackTime = Time.time + 1f / gunStats.fireRate;
        HandleShoot();
        onShootEvent.Invoke();
        return true;
    }

    protected virtual IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        transform.GetComponent<SpriteRenderer>().color = Color.green; // Change color to indicate reloading
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
        transform.GetComponent<SpriteRenderer>().color = Color.white; // Revert color after reloading
        isReloading = false;
    }

    public abstract void PrimaryAction();
    public abstract void SecondaryAction();
    public abstract void TertiaryAction();
    public abstract void HandleShoot();
}
