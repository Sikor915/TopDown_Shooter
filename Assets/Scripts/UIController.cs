using UnityEngine;
using TMPro;

public class UIController : Singleton<UIController>
{
    [SerializeField] TMP_Text ammoText, ammoReserveText;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject playerGO;
    [SerializeField] PlayerSO playerSO;
    [SerializeField] PlayerInventorySO playerInventorySO;

    PlayerController playerController;
    [SerializeField] Weapon weaponBase;

    void Awake()
    {
        playerController = playerGO.GetComponent<PlayerController>();
        weaponBase = playerInventorySO.currentWeapon.GetComponent<Weapon>();
    }

    void Start()
    {
        UpdateHealthText(playerSO.CurrentHealth, playerSO.MaxHealth);
        UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve);
        SetScore(0);
    }

    void OnEnable()
    {
        playerSO.onHealthChangedEvent.AddListener(UpdateHealthText);
        weaponBase.onShootEvent.AddListener(() => UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve));
        weaponBase.onReloadEvent.AddListener(() => UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve));
        playerInventorySO.onWeaponChangedEvent.AddListener(UpdateCurrentWeaponEvents);
    }

    public void UpdateAmmoText(int currentAmmo, int maxAmmo, int ammoReserve)
    {
        ammoReserveText.text = ammoReserve.ToString();
        ammoText.text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
    }

    public void UpdateHealthText(float currentHealth, float maxHealth)
    {
        Debug.Log("Updating health text");
        healthText.text = currentHealth.ToString("F0") + "/" + maxHealth.ToString("F0");
    }

    public void SetScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

    void UpdateCurrentWeaponEvents()
    {
        weaponBase.onShootEvent.RemoveAllListeners();
        weaponBase.onReloadEvent.RemoveAllListeners();
        weaponBase = playerInventorySO.currentWeapon.GetComponent<Weapon>();
        weaponBase.onShootEvent.AddListener(() => UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve));
        weaponBase.onReloadEvent.AddListener(() => UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve));
        UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve);
    }
}
