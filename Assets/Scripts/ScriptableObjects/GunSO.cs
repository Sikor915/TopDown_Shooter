using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "GunSO", menuName = "Scriptable Objects/GunSO")]
public class GunSO : ScriptableObject
{
    public string weaponName;
    public Sprite weaponIcon;
    public string weaponDescription;
    public int weaponPrice;
    public GameObject weaponPrefab;
    [Header("Stats")]
    public float baseDamage;
    public float baseFireRate;
    public int ammoReserve;
    [Description("In degrees, total spread angle")]
    public int baseSpread;
    public float baseProjectileSpeed;
    public float baseProjectileLifespan;

    [Description("Number of projectiles fired per attack")]
    public int baseProjectilesPerAttack;
    [Header("Reload Stats")]
    public float baseReloadTime;
    public int baseMagazineSize;
}