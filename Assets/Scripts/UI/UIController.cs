using UnityEngine;
using TMPro;
using System.Collections;

public class UIController : Singleton<UIController>
{
    [Header("Weapon UI Elements")]
    [SerializeField] TMP_Text ammoText;
    [SerializeField] TMP_Text ammoReserveText;
    [SerializeField] TMP_Text weaponNameText, previousWeaponNameText, nextWeaponNameText;

    [Header("Player UI Elements")]
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text moneyText, scoreText;
    [SerializeField] TMP_Text interactionPromptText;

    [Header("UI GameObjects")]
    [SerializeField] GameObject reloadProgressBar;
    [SerializeField] GameObject playerInteractProgressBar;
    [SerializeField] GameObject healthBar, lostHealthBar;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject gameOverScreen;

    [Header("References")]
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

    void Update()
    {
        if (PlayerInteractManager.Instance.IsPlayerNearInteractable())
        {
            interactionPromptText.gameObject.SetActive(true);
        }
        else
        {
            interactionPromptText.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (weaponBase != null)
        {
            weaponBase.onShootEvent.AddListener(() => UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve));
            weaponBase.onReloadEvent.AddListener(UpdateAmmoText);
        }
        PlayerController.Instance.onPlayerDeath.AddListener(PlayerDead);
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
        PlayerController.Instance.onPlayerDeath.RemoveListener(PlayerDead);
        playerSO.creatureSO.onHealthChangedEvent.RemoveListener(UpdateHealthText);
        playerInventory.onWeaponChangedEvent.RemoveListener(UpdateCurrentWeaponEvents);
        MoneyManager.Instance.ScoreSO.onScoreChangedEvent.RemoveListener(SetScore);
        MoneyManager.Instance.ScoreSO.onMoneyChangedEvent.RemoveListener(UpdateMoneyText);
    }

    public void RunReloadProgressBar(float reloadTime)
    {
        reloadProgressBar.SetActive(true);
        ProgressBar pb = reloadProgressBar.GetComponent<ProgressBar>();
        pb.SetProgress(1f, 1f / reloadTime);
    }

    public void StopReloadProgressBar()
    {
        reloadProgressBar.GetComponent<ProgressBar>().StopAllCoroutines();
        reloadProgressBar.SetActive(false);
    }

    public void RunPlayerInteractProgressBar(float interactTime = 0.4f)
    {
        playerInteractProgressBar.SetActive(true);
        ProgressBar pb = playerInteractProgressBar.GetComponent<ProgressBar>();
        pb.SetProgress(1f, 1f / interactTime);
    }

    public void StopPlayerInteractProgressBar()
    {
        playerInteractProgressBar.GetComponent<ProgressBar>().StopAllCoroutines();
        playerInteractProgressBar.SetActive(false);
    }

    public void UpdateAmmoText(int currentAmmo, int maxAmmo, int ammoReserve)
    {
        if (!weaponBase.usesAmmo)
        {
            ammoText.text = null;
            ammoReserveText.text = null;
            return;
        }
        StopReloadProgressBar();
        ammoReserveText.text = ammoReserve.ToString();
        ammoText.text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
    }

    public void UpdateWeaponNameText(string weaponName)
    {
        weaponNameText.text = weaponName;
    }

    public void UpdatePreviousWeaponNameText(string weaponName)
    {
        previousWeaponNameText.text = weaponName;
    }

    public void UpdateNextWeaponNameText(string weaponName)
    {
        nextWeaponNameText.text = weaponName;
    }

    public void UpdateHealthText(float currentHealth, float maxHealth)
    {
        healthText.text = currentHealth.ToString("F0") + "/" + maxHealth.ToString("F0");
        UpdateHealthBar(currentHealth, maxHealth);
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
        ammoReserveText.text = null;
        weaponNameText.text = null;
        previousWeaponNameText.text = null;
        nextWeaponNameText.text = null;
        gameOverScreen.SetActive(true);
    }

    public void ToggleSettingsMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
        if (settingsMenu.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthBar == null || lostHealthBar == null) return;
        float healthPercent = currentHealth / maxHealth;
        healthBar.GetComponent<ProgressBar>().SetProgress(healthPercent, 1f, false);
        lostHealthBar.GetComponent<ProgressBar>().SetProgress(1f - healthPercent, 0.5f, false);
    }

    void UpdateCurrentWeaponEvents()
    {
        if (weaponBase == null) return;
        weaponBase.onShootEvent.RemoveAllListeners();
        weaponBase.onReloadEvent.RemoveAllListeners();
        weaponBase = playerInventory.currentWeapon.GetComponent<Weapon>();
        weaponBase.onShootEvent.AddListener(() => UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve));
        weaponBase.onReloadEvent.AddListener(UpdateAmmoText);
        UpdateWeaponUIElements();
    }

    void UpdateWeaponUIElements()
    {
        UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve);
        UpdateWeaponNameText(weaponBase.gunStats.weaponName);
        GameObject previousWeapon = PlayerInventory.Instance.GetPreviousWeapon();
        GameObject nextWeapon = PlayerInventory.Instance.GetNextWeapon();

        if (previousWeapon != null)
        {
            UpdatePreviousWeaponNameText(previousWeapon.GetComponent<Weapon>().gunStats.weaponName);
        }
        else
        {
            UpdatePreviousWeaponNameText("");
        }
        if (nextWeapon != null)
        {
            UpdateNextWeaponNameText(nextWeapon.GetComponent<Weapon>().gunStats.weaponName);
        }
        else
        {
            UpdateNextWeaponNameText("");
        }
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
