using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "GunSO", menuName = "Scriptable Objects/GunSO")]
public class GunSO : ScriptableObject
{
    public string weaponName;
    [Header("Stats")]
    public float damage;
    public float fireRate;
    public int ammoReserve;
    [Description("In degrees, total spread angle")]
    public int spread;

    [Description("Number of projectiles fired per attack")]
    public int projectilesPerAttack;
    [Header("Reload Stats")]
    public float reloadTime;
    public int magazineSize;
}