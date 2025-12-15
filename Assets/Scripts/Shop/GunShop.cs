using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GunShop : Singleton<GunShop>, IShop
{
    [SerializeField] int gunsToShow = 3;
    public bool isOpen = false;


#if UNITY_EDITOR
    const string GunsPath = "Assets/Resources/GunData";
#else
    const string GunsPath = "GunData";
#endif
    List<GunSO> gunsRandomized = new();
    public List<GunSO> GunsRandomized => gunsRandomized;

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

    public bool TryBuyGun(GunSO gun)
    {
        if (MoneyManager.Instance.GetMoney() < gun.weaponPrice)
        {
            Debug.Log("Not enough money to buy " + gun.weaponName);
            return false;
        }
        MoneyManager.Instance.RemoveMoney(gun.weaponPrice);
        Vector3 weaponSpawnPosition = PlayerController.Instance.transform.position + new Vector3(Random.Range(1f, 3f), Random.Range(1f, 3f), 0);
        GameObject createdGun = Instantiate(gun.weaponPrefab, weaponSpawnPosition, Quaternion.identity);
        createdGun.SetActive(true);
        createdGun.GetComponent<Weapon>().DropWeaponPrepare();
        BoxCollider2D weaponCollider = createdGun.AddComponent<BoxCollider2D>();
        weaponCollider.size = new Vector2(1.0f, 1.0f);
        weaponCollider.isTrigger = true;

        RemoveGunFromShop(gun);
        Debug.Log("Gun bought: " + gun.weaponName);
        return true;
    }

    void RemoveGunFromShop(GunSO gun)
    {
        if (gunsRandomized.Contains(gun))
        {
            gunsRandomized.Remove(gun);
        }
    }

    void RandomizeGunsToShow()
    {
        List<GunSO> allGuns = HelperFunctions.GetScriptableObjectsOfType<GunSO>(GunsPath);
        if (allGuns.Count == 0)
        {
            Debug.LogError("No guns found in the specified path: " + GunsPath);
            string path = Application.dataPath + "/Resources/GunData";
            using (StreamWriter sw = File.CreateText(path + "/NewTextFile.txt"))
            {
                sw.WriteLine("This is a new text file!");
            }
            return;
        }
        gunsRandomized = new();

        while (gunsRandomized.Count < gunsToShow)
        {
            int randomIndex = Random.Range(0, allGuns.Count);
            GunSO randomGun = allGuns[randomIndex];
            if (!gunsRandomized.Contains(randomGun))
            {
                gunsRandomized.Add(randomGun);
            }
        }
        Debug.Log("Randomized guns to show in Gun Shop:");
        foreach (var gun in gunsRandomized)
        {
            Debug.Log(gun.weaponName);
        }
    }
}
