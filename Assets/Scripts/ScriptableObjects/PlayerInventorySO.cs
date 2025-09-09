using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Scriptable Objects/PlayerInventory", order = 1)]
public class PlayerInventorySO : ScriptableObject
{
    [Header("Events")]
    public UnityEvent onWeaponChangedEvent;

    [Header("Weapon Inventory")]
    public GameObject currentWeapon;
    [SerializeField] List<GameObject> weapons;
    public List<GameObject> Weapons => weapons;

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count) return;
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
        }
        currentWeapon = weapons[index];
        currentWeapon.SetActive(true);
        onWeaponChangedEvent?.Invoke();
    }

    public void EquipNextWeapon()
    {
        if (weapons.Count == 0) return;
        int currentIndex = weapons.IndexOf(currentWeapon);
        int nextIndex = (currentIndex + 1) % weapons.Count;
        EquipWeapon(nextIndex);
    }

    public void EquipPreviousWeapon()
    {
        if (weapons.Count == 0) return;
        int currentIndex = weapons.IndexOf(currentWeapon);
        int previousIndex = (currentIndex - 1 + weapons.Count) % weapons.Count;
        EquipWeapon(previousIndex);
    }

    public void AddWeapon(GameObject newWeapon)
    {
        if (!weapons.Contains(newWeapon))
        {
            weapons.Add(newWeapon);
            newWeapon.SetActive(false);
            Debug.Log("Added weapon: " + newWeapon.GetComponent<Weapon>().gunStats.weaponName);
        }
        else
        {
            Debug.Log("Weapon already in inventory.");
        }
    }

    public void RemoveWeapon(GameObject weaponToRemove)
    {
        if (weapons.Contains(weaponToRemove))
        {
            if (currentWeapon == weaponToRemove)
            {
                currentWeapon.SetActive(false);
                currentWeapon = null;
            }
            weapons.Remove(weaponToRemove);
            Debug.Log("Removed weapon: " + weaponToRemove.GetComponent<Weapon>().gunStats.weaponName);
        }
        else
        {
            Debug.Log("Weapon not found in inventory.");
        }
    }

    public void DropWeapon(GameObject weaponToDrop, Vector3 dropPosition)
    {
        if (weapons.Contains(weaponToDrop))
        {
            RemoveWeapon(weaponToDrop);
            weaponToDrop.GetComponent<Weapon>().DropWeaponPrepare();
            weaponToDrop.transform.position = dropPosition;
            BoxCollider2D weaponCollider = weaponToDrop.AddComponent<BoxCollider2D>();
            weaponCollider.size = new Vector2(1.0f, 1.0f);
            weaponCollider.isTrigger = true;

            EquipNextWeapon();
            Debug.Log("Dropped weapon: " + weaponToDrop.GetComponent<Weapon>().gunStats.weaponName);
        }
        else
        {
            Debug.Log("Weapon not found in inventory.");
        }
    }

    public void PickUpWeapon(GameObject weaponToPickUp, Transform parent)
    {
        if (!weapons.Contains(weaponToPickUp))
        {
            AddWeapon(weaponToPickUp);
            weaponToPickUp.GetComponent<Weapon>().PickUpWeaponPrepare(parent);
            Destroy(weaponToPickUp.GetComponent<BoxCollider2D>());
            EquipWeapon(weapons.IndexOf(weaponToPickUp));
            if (weaponToPickUp.name != "Katana")
            {
                weaponToPickUp.transform.SetLocalPositionAndRotation(new Vector3(1.51f, 0, 0), Quaternion.Euler(0, 0, 90));
            }
            else
            {
                weaponToPickUp.transform.SetLocalPositionAndRotation(new Vector3(-0.65f, -0.11f, 0), Quaternion.Euler(0, 0, -125.3f));
            }
            Debug.Log("Picked up weapon: " + weaponToPickUp.GetComponent<Weapon>().gunStats.weaponName);
        }
        else
        {
            Debug.Log("Weapon already in inventory.");
        }
    }

    public GameObject GetWeaponByName(string weaponName)
    {
        return weapons.Find(w => w.GetComponent<Weapon>().gunStats.weaponName == weaponName);
    }
}
