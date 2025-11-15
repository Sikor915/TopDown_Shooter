using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GunPanel : MonoBehaviour
{
    [SerializeField] TMP_Text gunNameText;
    [SerializeField] Image gunIcon;
    [SerializeField] TMP_Text gunDescriptionText;
    [SerializeField] TMP_Text gunPriceText;
    [SerializeField] Button buyButton;

    public GunSO gunData;

    void OnEnable()
    {
        buyButton.onClick.AddListener(() => { GunShopUI.Instance.onGunBought?.Invoke(this); });
    }

    void OnDisable()
    {
        buyButton.onClick.RemoveAllListeners();
    }

    public void SetGunPanel(string gunName, Sprite gunIcon, string gunDescription, int gunPrice, GunSO gunData = null)
    {
        this.gunData = gunData;
        gunNameText.text = gunName;
        this.gunIcon.sprite = gunIcon;
        gunDescriptionText.text = gunDescription;
        gunPriceText.text = "Price: " + gunPrice.ToString() + " $";
    }
}