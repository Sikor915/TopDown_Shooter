using UnityEngine;
using TMPro;

public class UIController : Singleton<UIController>
{
    [SerializeField] TMP_Text ammoText, ammoReserveText;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] GameObject playerGO;
    [SerializeField] PlayerSO playerSO;

    PlayerController playerController;
    Weapon weaponBase;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = playerGO.GetComponent<PlayerController>();
        weaponBase = playerGO.GetComponentInChildren<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        playerSO.onHealthChangedEvent.AddListener(UpdateHealthText);
        weaponBase.onShootEvent.AddListener(() => UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve));
        weaponBase.onReloadEvent.AddListener(() => UpdateAmmoText(weaponBase.CurrentAmmo, weaponBase.gunStats.magazineSize, weaponBase.gunStats.ammoReserve));
    }

    public void UpdateAmmoText(int currentAmmo, int maxAmmo, int ammoReserve)
    {
        ammoReserveText.text = ammoReserve.ToString();
        ammoText.text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
    }

    public void UpdateHealthText(float currentHealth, float maxHealth)
    {
        Debug.Log("Updating health text");
        healthText.text = currentHealth.ToString("F1") + "/" + maxHealth.ToString("F1");
    }

    public void SetScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }
}
