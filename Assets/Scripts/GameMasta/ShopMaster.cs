using System.Collections.Generic;
using UnityEngine;

class ShopMaster : Singleton<ShopMaster>
{
    [SerializeField] PlayerController pc;
    [SerializeField] GameObject exitPoint;
    public GameObject ExitPoint => exitPoint;
    [SerializeField] PerkShop perkShopPoint;
    public PerkShop PerkShopPoint => perkShopPoint;
    [SerializeField] GunShop gunShopPoint;
    public GunShop GunShopPoint => gunShopPoint;

    

    void Awake()
    {
        pc = PlayerController.Instance;
        pc.gameObject.transform.position = exitPoint.transform.position;
    }

    void Start()
    {
        pc.PlayerSO.onHealthChangedEvent?.Invoke(pc.CurrentHealth, pc.PlayerSO.MaxHealth);
    }

    
}