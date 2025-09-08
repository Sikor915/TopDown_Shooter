using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeSO", menuName = "Scriptable Objects/MeleeSO")]
public class MeleeSO : ScriptableObject
{
    public string weaponName;
    [Header("Stats")]
    public float damage;
    public float attackSpeed;

}