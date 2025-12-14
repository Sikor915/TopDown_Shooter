using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public abstract class Weapon : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent onShootEvent;
    public UnityEvent<int, int, int> onReloadEvent;

    [Header("References")]
    [SerializeField] GunSO gunBaseStats;
    [SerializeField] protected AudioClip shootSound;
    public GameObject projectilePrefab;
    public CreatureSO ownerCreatureSO;
    [SerializeField] protected Animator anim;
    [SerializeField] protected AudioMixerGroup audioMixerGroup;

    [Header("Weapon Info")]
    public bool usesAmmo = true;
    public bool isUsedByPlayer = false;
    public bool isMeleeWeapon = false;

    public struct WeaponStats
    {
        public string weaponName;
        public float damage;
        public float thrownDamage;
        public float fireRate;
        public int ammoReserve;
        public int spread;
        public float projectileSpeed;
        public float projectileLifespan;
        public int projectilesPerAttack;
        public float reloadTime;
        public int magazineSize;
    }
    public WeaponStats gunStats;

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

    bool isBeingThrown;
    public bool IsBeingThrown
    {
        get { return isBeingThrown; }
        set { isBeingThrown = value; }
    }

    protected SpriteRenderer spriteRenderer;

    protected void Awake()
    {
        CopyFromSO();
        onShootEvent ??= new UnityEvent();
        onReloadEvent ??= new UnityEvent<int, int, int>();
        currentAmmo = gunStats.magazineSize;
        if (projectilePrefab.TryGetComponent<BooletController>(out var booletC))
        {
            booletC.Damage = gunStats.damage;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Update()
    {
        // This can be overridden in derived classes if needed
    }

    void OnDisable()
    {
        isReloading = false;
        StopAllCoroutines();
        PlayerAim.Instance.onFacingDirectionChanged.RemoveListener(CorrectSpriteGraphics);
        ownerCreatureSO.onStatsChangedEvent.RemoveListener(CalculateUpgradableStats);
    }

    void OnEnable()
    {
        PlayerAim.Instance.onFacingDirectionChanged.AddListener(CorrectSpriteGraphics);
        ownerCreatureSO.onStatsChangedEvent.AddListener(CalculateUpgradableStats);
        CalculateUpgradableStats();
    }

    void CopyFromSO()
    {
        gunStats.weaponName = gunBaseStats.weaponName;
        gunStats.damage = gunBaseStats.baseDamage;
        gunStats.thrownDamage = gunBaseStats.baseThrownDamage;
        gunStats.fireRate = gunBaseStats.baseFireRate;
        gunStats.ammoReserve = gunBaseStats.ammoReserve;
        gunStats.spread = gunBaseStats.baseSpread;
        gunStats.projectileSpeed = gunBaseStats.baseProjectileSpeed;
        gunStats.projectileLifespan = gunBaseStats.baseProjectileLifespan;
        gunStats.projectilesPerAttack = gunBaseStats.baseProjectilesPerAttack;
        gunStats.reloadTime = gunBaseStats.baseReloadTime;
        gunStats.magazineSize = gunBaseStats.baseMagazineSize;
    }

    virtual public void TryReload()
    {
        if (isReloading || gunStats.ammoReserve <= 0 || currentAmmo == gunStats.magazineSize) return;
        if (isUsedByPlayer)
        {
            UIController.Instance.RunReloadProgressBar(gunStats.reloadTime);
        }
        StartCoroutine(ReloadCoroutine());
    }

    public virtual bool TryShoot()
    {
        if (isReloading || Time.time < nextAttackTime || currentAmmo <= 0) return false;
        if ((PerkShop.Instance != null && PerkShop.Instance.isOpen) || (GunShop.Instance != null && GunShop.Instance.isOpen) || (MainMenuController.Instance != null)) return false;

        // Shoot
        nextAttackTime = Time.time + 1f / gunStats.fireRate;
        HandleShoot();
        onShootEvent.Invoke();
        return true;
    }

    protected virtual IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(gunStats.reloadTime);

        if (!isUsedByPlayer)
        {
            currentAmmo = gunStats.magazineSize;
            gunStats.ammoReserve = gunStats.magazineSize;
        }
        else
        {
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
        }
        onReloadEvent.Invoke(currentAmmo, gunStats.magazineSize, gunStats.ammoReserve);
        Debug.Log("Reloaded. Current ammo: " + currentAmmo + ", Ammo reserve: " + gunStats.ammoReserve);
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
        Light2D weaponLight = gameObject.AddComponent<Light2D>();
        weaponLight.intensity = 2.0f;
        weaponLight.pointLightOuterRadius = 3.0f;
    }

    public void DropWeapon(Vector3 dropPosition)
    {
        transform.position = dropPosition;

        BoxCollider2D weaponCollider = gameObject.AddComponent<BoxCollider2D>();
        weaponCollider.size = new Vector2(1.0f, 1.0f);
        weaponCollider.isTrigger = true;
    }

    public void PickUpWeaponPrepare(Transform parent)
    {
        transform.SetParent(parent);
        if (isMeleeWeapon && TryGetComponent<Katana>(out var katana))
        {
            (Vector3 basePos, Quaternion baseRot) = katana.GetBaseTransform();
            transform.SetLocalPositionAndRotation(basePos, baseRot);
        }
        else
        {
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
        if (TryGetComponent<Light2D>(out var light2d))
        {
            Destroy(light2d);
        }
        gameObject.GetComponent<Weapon>().enabled = true;
    }

    public virtual void CalculateUpgradableStats()
    {
        gunStats.fireRate = gunBaseStats.baseFireRate * (1 + ownerCreatureSO.GetStat(StatInfo.Stat.PercentBonusFireRate));
        gunStats.reloadTime = gunBaseStats.baseReloadTime * (1 - ownerCreatureSO.GetStat(StatInfo.Stat.PercentBonusReloadSpeed));
        gunStats.projectileSpeed = gunBaseStats.baseProjectileSpeed * (1 + ownerCreatureSO.GetStat(StatInfo.Stat.PercentBonusProjectileSpeed));
        gunStats.projectileLifespan = gunBaseStats.baseProjectileLifespan * (1 + ownerCreatureSO.GetStat(StatInfo.Stat.PercentBonusProjectileLifespan));
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

    protected virtual void CorrectSpriteGraphics(PlayerAim.FacingDirection facingDir)
    {
        if (facingDir == PlayerAim.FacingDirection.NW || facingDir == PlayerAim.FacingDirection.W || facingDir == PlayerAim.FacingDirection.SW)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBeingThrown)
        {
            Debug.Log("Thrown weapon collided with: " + collision.collider.name);
            if (collision.collider.CompareTag("Enemy"))
            {
                collision.collider.GetComponent<IEnemy>().DeductHealth(gunStats.thrownDamage);
                Debug.Log("Thrown weapon hit enemy for " + gunStats.thrownDamage + " damage.");

            }
            Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                rb2d.linearVelocity = Vector2.zero;
                rb2d.angularVelocity = 0;
                Destroy(rb2d);
            }
            BoxCollider2D weaponCollider = GetComponent<BoxCollider2D>();
            if (weaponCollider != null)
            {
                Destroy(weaponCollider);
            }
            isBeingThrown = false;

            DropWeapon(transform.position);
        }
    }

    public abstract void PrimaryAction();
    public abstract void SecondaryAction();
    public abstract void TertiaryAction();
    public abstract void ReloadAction();
    public abstract void BotUse();
    public abstract void HandleShoot();

}
