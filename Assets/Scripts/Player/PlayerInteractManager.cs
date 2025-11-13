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
            Debug.Log("Picking up weapon: " + nearestWeapon.name);
            playerInventory.PickUpWeapon(nearestWeapon, transform);
            playerInventory.currentWeapon.GetComponent<Weapon>().ownerCreatureSO = playerSO.creatureSO;
            playerInventory.currentWeapon.GetComponent<Weapon>().CalculateUpgradableStats();
            return;
        }
        if (MapGenerator.Instance != null)
        {
            Room endRoom = MapGenerator.Instance.GetStartAndEndRooms().Item2;
            if (endRoom != null && MapGenerator.Instance.IsPlayerInRoom(endRoom))
            {
                Debug.Log("Player interacted in end room. Loading Main Menu.");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Shop");
                return;
            }
        }
        if (ShopMaster.Instance != null)
        {
            Debug.Log("Checking shop exit point interaction");
            float distance = Vector3.Distance(transform.position, ShopMaster.Instance.ExitPoint.transform.position);
            if (distance < 5.0f)
            {
                Debug.Log("Player interacted at shop exit point. Loading Main Menu.");
                UnityEngine.SceneManagement.SceneManager.LoadScene("DemoScene");
                return;
            }
            Debug.Log($"Player not close enough to exit point. Distance was: {distance}");

            distance = Vector3.Distance(transform.position, ShopMaster.Instance.PerkShopPoint.transform.position);
            if (distance < 4.0f)
            {
                Debug.Log("Player interacted at perk shop point. Opening Perk Shop.");
                ShopMaster.Instance.PerkShopPoint.OpenShop();
                return;
            }

            distance = Vector3.Distance(transform.position, ShopMaster.Instance.WeaponShopPoint.transform.position);
            if (distance < 4.0f)
            {
                Debug.Log("Player interacted at weapon shop point. Opening Weapon Shop.");
                ShopMaster.Instance.WeaponShopPoint.OpenShop();
                return;
            }

        }
    }
}