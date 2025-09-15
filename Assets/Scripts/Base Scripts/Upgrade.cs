using UnityEngine;
using AYellowpaper.SerializedCollections;

public abstract class Upgrade : ScriptableObject
{
    public string upgradeName;
    public string upgradeDescription;
    [SerializedDictionary("Stat", "Value")]
    public SerializedDictionary<StatInfo.Stat, float> affectedStats = new();

    public abstract void ApplyUpgrade();
}