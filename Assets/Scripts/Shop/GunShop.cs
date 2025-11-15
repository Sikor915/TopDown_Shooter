using System.Collections.Generic;
using UnityEngine;

public class GunShop : Singleton<GunShop>, IShop
{
    [SerializeField] int gunsToShow = 3;
    public bool isOpen = false;
    
    const string GunsPath = "Assets/ScriptableObjects/GunData";

    
    List<GunSO> gunsRandomized = new();

    void Awake()
    {
        RandomizeGunsToShow();
    }

    public void OpenShop()
    {
        GunShopUI.Instance.ShowUI();
        isOpen = true;
    }

    public void CloseShop()
    {
        GunShopUI.Instance.HideUI();
        isOpen = false;
    }

    void RandomizeGunsToShow()
    {
        List<GunSO> allGuns = HelperFunctions.GetScriptableObjectsOfType<GunSO>(GunsPath);

        while (gunsRandomized.Count < gunsToShow)
        {
            int randomIndex = Random.Range(0, allGuns.Count);
            GunSO randomUpgrade = allGuns[randomIndex];
            if (!gunsRandomized.Contains(randomUpgrade))
            {
                gunsRandomized.Add(randomUpgrade);
            }
        }
        Debug.Log("Randomized guns to show in Gun Shop:");
        foreach (var gun in gunsRandomized)
        {
            Debug.Log(gun.weaponName);
        }
    }
}
