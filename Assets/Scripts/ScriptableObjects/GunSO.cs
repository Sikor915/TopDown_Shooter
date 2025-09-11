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
    public float projectileSpeed;
    public float projectileLifespan;

    [Description("Number of projectiles fired per attack")]
    public int projectilesPerAttack;
    [Header("Reload Stats")]
    public float reloadTime;
    public int magazineSize;

    [Header("Base Stats")]
    public float baseDamage;
    public float baseFireRate;
    public int baseMagazineSize;
    public float baseReloadTime;
    public float baseSpread;
    public float baseProjectileSpeed;
    public float baseProjectileLifespan;
    public int baseProjectilesPerAttack;
}