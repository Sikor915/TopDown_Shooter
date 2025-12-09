using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

class PlayerInventory : Singleton<PlayerInventory>
{
    [Header("Events")]
    public UnityEvent onWeaponChangedEvent = new();

    [Header("Weapon Inventory")]
    public GameObject currentWeapon;
    public bool HasCurrentWeapon => currentWeapon != null;
    [SerializeField] List<GameObject> weapons;
    public List<GameObject> Weapons => weapons;

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count) return;
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
            Debug.Log("Disabled current weapon");
        }
        currentWeapon = weapons[index];
        currentWeapon.SetActive(true);
        UIController.Instance.StopReloadProgressBar();
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
        }
        else
        {
            Debug.LogWarning("Weapon already in inventory.");
        }
        newWeapon.GetComponent<Weapon>().isUsedByPlayer = true;
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
            weaponToRemove.GetComponent<Weapon>().isUsedByPlayer = false;
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
            weaponToDrop.GetComponent<Weapon>().DropWeapon(dropPosition);

            EquipNextWeapon();
        }
        else
        {
            Debug.Log("Weapon not found in inventory.");
        }
    }

    public void ThrowCurrentWeapon(Vector3 throwPoint, float throwForce)
    {
        if (currentWeapon != null)
        {
            GameObject weaponToThrow = currentWeapon;
            RemoveWeapon(weaponToThrow);

            weaponToThrow.GetComponent<Weapon>().DropWeaponPrepare();
            weaponToThrow.GetComponent<Weapon>().IsBeingThrown = true;
            if (weaponToThrow.TryGetComponent<PolygonCollider2D>(out var polyCollider))
            {
                Debug.Log("PolygonCollider2D found on thrown weapon.");
                weaponToThrow.transform.SetPositionAndRotation(new Vector3(3,1,0), Quaternion.Euler(0, 0, 30));
                polyCollider.enabled = true;
            }
            else
            {
                weaponToThrow.transform.position = transform.position + (throwPoint - transform.position).normalized * 2f;
                BoxCollider2D weaponCollider = weaponToThrow.AddComponent<BoxCollider2D>();
                weaponCollider.size = new Vector2(1.0f, 1.0f);
            }
            Debug.Log("Throwing weapon at position: " + throwPoint);

            Rigidbody2D rb2d = weaponToThrow.AddComponent<Rigidbody2D>();
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            rb2d.gravityScale = 0;
            rb2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb2d.linearVelocity = Vector2.zero;
            rb2d.angularVelocity = 0;

            Debug.Log("RB2D added to thrown weapon.");

            Vector3 throwDirection = (throwPoint - weaponToThrow.transform.position).normalized;

            rb2d.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);

            EquipNextWeapon();
            Debug.Log("Threw weapon: " + weaponToThrow.GetComponent<Weapon>().gunStats.weaponName);
        }
        else
        {
            Debug.Log("No current weapon to throw.");
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
                weaponToPickUp.transform.SetLocalPositionAndRotation(new Vector3(1.51f, 0, 0), Quaternion.identity);
            }
            else
            {
                weaponToPickUp.transform.SetLocalPositionAndRotation(new Vector3(-0.65f, -0.11f, 0), Quaternion.Euler(0, 0, -125.3f));
            }
        }
        else
        {
            Debug.Log("Weapon already in inventory.");
        }
    }

    public GameObject GetPreviousWeapon()
    {
        if (weapons.Count == 0 || currentWeapon == null) return null;
        int currentIndex = weapons.IndexOf(currentWeapon);
        int previousIndex = (currentIndex - 1 + weapons.Count) % weapons.Count;
        return weapons[previousIndex];
    }

    public GameObject GetNextWeapon()
    {
        if (weapons.Count == 0 || currentWeapon == null) return null;
        int currentIndex = weapons.IndexOf(currentWeapon);
        int nextIndex = (currentIndex + 1) % weapons.Count;
        return weapons[nextIndex];
    }

    public GameObject GetWeaponByName(string weaponName)
    {
        return weapons.Find(w => w.GetComponent<Weapon>().gunStats.weaponName == weaponName);
    }
}