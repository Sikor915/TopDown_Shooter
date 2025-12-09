using UnityEngine;

class PlayerInteractManager : Singleton<PlayerInteractManager>
{

    PlayerInventory playerInventory;
    PlayerSO playerSO;
    GameObject nearestWeapon;
    public GameObject NearestWeapon
    {
        get => nearestWeapon;
        set => nearestWeapon = value;
    }
    bool canPickUp;
    public bool CanPickUp
    {
        get => canPickUp;
        set => canPickUp = value;
    }
    void Awake()
    {
        playerInventory = PlayerController.Instance.GetComponent<PlayerInventory>();
        playerSO = PlayerController.Instance.PlayerSO;
    }
    public void ProcessInteraction()
    {
        Debug.Log("Interact pressed");
        if (canPickUp)
        {
            playerInventory.PickUpWeapon(nearestWeapon, transform.GetChild(0));
            playerInventory.currentWeapon.GetComponent<Weapon>().ownerCreatureSO = playerSO.creatureSO;
            playerInventory.currentWeapon.GetComponent<Weapon>().CalculateUpgradableStats();
            return;
        }
        if (MapGenerator.Instance != null)
        {
            if (CheckInteractionInEndRoom())
            {
                Debug.Log("Player interacted in end room. Loading Shop.");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Shop");
                return;
            }
        }
        if (ShopMaster.Instance != null)
        {
            Debug.Log("Checking shop exit point interaction");

            if (CheckInteractionsInShop())
            {
                return;
            }
        }
    }

    public bool IsPlayerNearInteractable()
    {
        Vector2Int playerPos = new Vector2Int((int)PlayerController.Instance.transform.position.x, (int)PlayerController.Instance.transform.position.y);
        return canPickUp || 
        (MapGenerator.Instance != null && MapGenerator.Instance.GetCurrentRoom(playerPos) == MapGenerator.Instance.GetStartAndEndRooms().Item2) || 
        (ShopMaster.Instance != null && IsNearAnythingInShop());
    }

    bool CheckInteractionInEndRoom()
    {
        if (MapGenerator.Instance != null)
        {
            Room endRoom = MapGenerator.Instance.GetStartAndEndRooms().Item2;
            if (endRoom != null && MapGenerator.Instance.IsPlayerInRoom(endRoom))
            {
                return true;
            }
        }
        return false;
    }

    bool IsNearAnythingInShop()
    {
        return IsNearPoint(ShopMaster.Instance.ExitPoint.transform.position, 5.0f) ||
               IsNearPoint(ShopMaster.Instance.PerkShopPoint.transform.position, 4.0f) ||
               IsNearPoint(ShopMaster.Instance.GunShopPoint.transform.position, 4.0f);
    }

    bool IsNearPoint(Vector3 point, float threshold)
    {
        float distance = Vector3.Distance(transform.position, point);
        return distance < threshold;
    }

    bool CheckInteractionsInShop()
    {
        if (IsNearPoint(ShopMaster.Instance.ExitPoint.transform.position, 5.0f))
        {
            Debug.Log("Player interacted at shop exit point. Loading Main Menu.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("DemoScene");
            return true;
        }

        if (IsNearPoint(ShopMaster.Instance.PerkShopPoint.transform.position, 4.0f))
        {
            Debug.Log("Player interacted at perk shop point. Opening Perk Shop.");
            ShopMaster.Instance.PerkShopPoint.OpenShop();
            return true;
        }

        if (IsNearPoint(ShopMaster.Instance.GunShopPoint.transform.position, 4.0f))
        {
            Debug.Log("Player interacted at gun shop point. Opening Gun Shop.");
            ShopMaster.Instance.GunShopPoint.OpenShop();
            return true;
        }
        return false;
    }
}