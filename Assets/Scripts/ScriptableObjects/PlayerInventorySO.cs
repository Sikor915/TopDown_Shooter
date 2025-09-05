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
            newWeapon.SetActive(false); // Ensure the weapon is inactive when added
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

    public GameObject GetWeaponByName(string weaponName)
    {
        return weapons.Find(w => w.GetComponent<Weapon>().gunStats.weaponName == weaponName);
    }
}
