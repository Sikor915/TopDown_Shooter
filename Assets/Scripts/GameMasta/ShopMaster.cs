using System.Collections.Generic;
using UnityEngine;

class ShopMaster : Singleton<ShopMaster>
{
    [SerializeField] PlayerController pc;
    [SerializeField] GameObject exitPoint;
    public GameObject ExitPoint => exitPoint;
    [SerializeField] PerkShop perkShopPoint;
    public PerkShop PerkShopPoint => perkShopPoint;
    [SerializeField] WeaponShop weaponShopPoint;
    public WeaponShop WeaponShopPoint => weaponShopPoint;

    

    void Awake()
    {
        pc = PlayerController.Instance;
        pc.gameObject.transform.position = exitPoint.transform.position;
    }

    void Start()
    {
        pc.PlayerSO.creatureSO.onHealthChangedEvent?.Invoke(pc.CurrentHealth, pc.PlayerSO.creatureSO.MaxHealth);
    }

    
}