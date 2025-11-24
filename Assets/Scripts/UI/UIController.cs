using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Collections;

public class UIController : Singleton<UIController>
{
    [SerializeField] TMP_Text ammoText, ammoReserveText;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] PlayerSO playerSO;
    [SerializeField] PlayerInventory playerInventory;

    [SerializeField] Weapon weaponBase;

    void Awake()
    {
        playerInventory = PlayerController.Instance.GetComponent<PlayerInventory>();
    }

    void Start()
    {
        StartCoroutine(TryGetWeaponBase());
        SetScore(MoneyManager.Instance.GetScore());
        UpdateMoneyText(MoneyManager.Instance.GetMoney());
    }

    void OnEnable()
    {
        if (weaponBase != null)
        {
            weaponBase.onShootEvent.AddListener(() => UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve));
            weaponBase.onReloadEvent.AddListener(UpdateAmmoText);
        }
        playerSO.creatureSO.onHealthChangedEvent.AddListener(UpdateHealthText);
        playerInventory.onWeaponChangedEvent.AddListener(UpdateCurrentWeaponEvents);
        MoneyManager.Instance.ScoreSO.onScoreChangedEvent.AddListener(SetScore);
        MoneyManager.Instance.ScoreSO.onMoneyChangedEvent.AddListener(UpdateMoneyText);
    }

    void OnDisable()
    {
        if (weaponBase != null)
        {
            weaponBase.onShootEvent.RemoveAllListeners();
            weaponBase.onReloadEvent.RemoveAllListeners();
        }
        playerSO.creatureSO.onHealthChangedEvent.RemoveListener(UpdateHealthText);
        playerInventory.onWeaponChangedEvent.RemoveListener(UpdateCurrentWeaponEvents);
        MoneyManager.Instance.ScoreSO.onScoreChangedEvent.RemoveListener(SetScore);
        MoneyManager.Instance.ScoreSO.onMoneyChangedEvent.RemoveListener(UpdateMoneyText);
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

    public void UpdateMoneyText(int currentMoney)
    {
        moneyText.text = "$" + currentMoney.ToString();
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
        if (weaponBase == null) return;
        weaponBase.onShootEvent.RemoveAllListeners();
        weaponBase.onReloadEvent.RemoveAllListeners();
        weaponBase = playerInventory.currentWeapon.GetComponent<Weapon>();
        weaponBase.onShootEvent.AddListener(() => UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve));
        weaponBase.onReloadEvent.AddListener(UpdateAmmoText);
        UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve);
    }

    IEnumerator TryGetWeaponBase()
    {
        while (weaponBase == null)
        {
            Debug.Log("Trying to get weapon base");
            if (playerInventory.HasCurrentWeapon)
            {
                weaponBase = playerInventory.currentWeapon.GetComponent<Weapon>();
            }
            yield return null;
        }
        UpdateCurrentWeaponEvents();
    }
}
