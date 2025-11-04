using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

public class UIController : Singleton<UIController>
{
    [SerializeField] TMP_Text ammoText, ammoReserveText;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject playerGO;
    [SerializeField] PlayerSO playerSO;
    [SerializeField] PlayerInventorySO playerInventorySO;

    [SerializeField] Weapon weaponBase;

    void Awake()
    {
        weaponBase = playerInventorySO.currentWeapon.GetComponent<Weapon>();
    }

    void Start()
    {
        StartCoroutine(TryGetWeaponBase());
        UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve);
        SetScore(0);
    }

    void OnEnable()
    {
        playerSO.creatureSO.onHealthChangedEvent.AddListener(UpdateHealthText);
        weaponBase.onShootEvent.AddListener(() => UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve));
        weaponBase.onReloadEvent.AddListener(UpdateAmmoText);
        playerInventorySO.onWeaponChangedEvent.AddListener(UpdateCurrentWeaponEvents);
    }

    public void UpdateAmmoText(int currentAmmo, int maxAmmo, int ammoReserve)
    {
        if (!weaponBase.usesAmmo)
        {
            ammoText.text = null;
            ammoReserveText.text = null;
            return;
        }
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

    public void PlayerDead()
    {
        healthText.text = null;
        ammoText.text = null;
    }

    void UpdateCurrentWeaponEvents()
    {
        weaponBase.onShootEvent.RemoveAllListeners();
        weaponBase.onReloadEvent.RemoveAllListeners();
        weaponBase = playerInventorySO.currentWeapon.GetComponent<Weapon>();
        weaponBase.onShootEvent.AddListener(() => UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve));
        weaponBase.onReloadEvent.AddListener(UpdateAmmoText);
        UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve);
    }

    IEnumerator TryGetWeaponBase()
    {
        while (weaponBase == null)
        {
            Debug.Log("Trying to get weapon base");
            weaponBase = playerInventorySO.currentWeapon.GetComponent<Weapon>();
            yield return null;
        }
        UpdateHealthText(GameMaster.Instance.PlayerController.CurrentHealth, playerSO.creatureSO.MaxHealth);
        UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve);
        
    }
}
