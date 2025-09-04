using UnityEngine;

[CreateAssetMenu(fileName = "GunSO", menuName = "Scriptable Objects/GunSO")]
public class GunSO : ScriptableObject
{
    public string weaponName;
    [Header("Stats")]
    public float damage;
    public float fireRate;
    public int ammoReserve;
    [Header("Reload Stats")]
    public float reloadTime;
    public int magazineSize;
}