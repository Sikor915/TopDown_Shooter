using UnityEngine;

public class WeaponShop : MonoBehaviour, IShop
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenShop()
    {
        Debug.Log("Weapon Shop opened");
    }
    
    public void CloseShop()
    {
        Debug.Log("Weapon Shop closed");
    }
}
