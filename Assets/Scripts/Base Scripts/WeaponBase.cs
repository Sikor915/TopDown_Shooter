using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class Weapon : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onShootEvent;
    public UnityEvent<int, int, int> onReloadEvent;

    [Header("References")]
    public GunSO gunStats;
    public GameObject projectilePrefab;
    public CreatureSO ownerCreatureSO;
    [SerializeField] protected Animator anim;

    [Header("Weapon Info")]
    public bool usesAmmo = true;

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
        onReloadEvent ??= new UnityEvent<int, int, int>();
        currentAmmo = gunStats.magazineSize;
        if (projectilePrefab.TryGetComponent<BooletController>(out var booletC))
        {
            booletC.Damage = gunStats.damage;
        }
    }

    public virtual void Update()
    {
        // This can be overridden in derived classes if needed
    }

    void OnDisable()
    {
        isReloading = false;
        StopAllCoroutines();
        ResetGraphics();
        ownerCreatureSO.onStatsChangedEvent.RemoveListener(CalculateUpgradableStats);
    }

    void OnEnable()
    {
        ownerCreatureSO.onStatsChangedEvent.AddListener(CalculateUpgradableStats);
        CalculateUpgradableStats();
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
        Color color = transform.GetComponent<SpriteRenderer>().color;
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
        onReloadEvent.Invoke(currentAmmo, gunStats.magazineSize, gunStats.ammoReserve);
        Debug.Log("Reloaded. Current ammo: " + currentAmmo + ", Ammo reserve: " + gunStats.ammoReserve);
        transform.GetComponent<SpriteRenderer>().color = color; // Revert color after reloading
        isReloading = false;
    }

    public void DropWeaponPrepare()
    {
        isReloading = false;
        StopAllCoroutines();
        transform.SetParent(null);
        gameObject.SetActive(true);
        gameObject.GetComponent<Weapon>().enabled = false;
        ownerCreatureSO = null;
    }

    public void PickUpWeaponPrepare(Transform parent)
    {
        transform.SetParent(parent);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        gameObject.GetComponent<Weapon>().enabled = true;
    }

    public virtual void CalculateUpgradableStats()
    {
        gunStats.fireRate = gunStats.baseFireRate * (1 + ownerCreatureSO.GetStat(StatInfo.Stat.PercentBonusFireRate));
        gunStats.reloadTime = gunStats.baseReloadTime * (1 - ownerCreatureSO.GetStat(StatInfo.Stat.PercentBonusReloadSpeed));
        gunStats.projectileSpeed = gunStats.baseProjectileSpeed * (1 + ownerCreatureSO.GetStat(StatInfo.Stat.PercentBonusProjectileSpeed));
        gunStats.projectileLifespan = gunStats.baseProjectileLifespan * (1 + ownerCreatureSO.GetStat(StatInfo.Stat.PercentBonusProjectileLifespan));
    }
    
    protected virtual float CalculateDamage()
    {
        float totalFlatDamage = gunStats.damage + (ownerCreatureSO.GetStat(StatInfo.Stat.BonusDamage) / gunStats.projectilesPerAttack);
        float totalDamage = totalFlatDamage * (1 + ownerCreatureSO.GetStat(StatInfo.Stat.PercentBonusDamage));
        bool isCrit = Random.value < ownerCreatureSO.GetStat(StatInfo.Stat.PercentCritChance);
        if (isCrit)
        {
            totalDamage *= (1 + ownerCreatureSO.GetStat(StatInfo.Stat.PercentCritDamage));
        }
        return totalDamage;
    }

    public abstract void PrimaryAction();
    public abstract void SecondaryAction();
    public abstract void TertiaryAction();
    public abstract void HandleShoot();
    public abstract void ResetGraphics();
}
