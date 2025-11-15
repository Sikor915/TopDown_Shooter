using UnityEngine;
using System;

public class GunShopUI : Singleton<GunShopUI>
{
    [SerializeField] GameObject GunPanelPrefab;
    [SerializeField] Transform gunPanelParent;
    [SerializeField] Transform closeButton;
    public Action<GunPanel> onGunBought;

    void OnDisable()
    {
        onGunBought = null;
    }

    void OnEnable()
    {
        onGunBought += (gunPanel) =>
        {
            if(GunShop.Instance.TryBuyGun(gunPanel.gunData)) Destroy(gunPanel.gameObject);
        };
        SetUpGunPanels();
    }

    public void ShowUI()
    {
        gunPanelParent.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gunPanelParent.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

    void SetUpGunPanels()
    {
        foreach (Transform child in gunPanelParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var gun in GunShop.Instance.GunsRandomized)
        {
            GunPanel panel = Instantiate(GunPanelPrefab, gunPanelParent).GetComponent<GunPanel>();
            panel.SetGunPanel(gun.weaponName, gun.weaponIcon, gun.weaponDescription, gun.weaponPrice, gun);
        }
    }
}