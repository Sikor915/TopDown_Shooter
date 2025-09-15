[System.Serializable]
public class StatInfo
{
    public enum Stat
    {
        BonusDamage,
        PercentBonusDamage,
        PercentCritChance,
        PercentCritDamage,
        PercentBonusReloadSpeed,
        PercentBonusFireRate,
        PercentBonusProjectileSpeed,
        PercentBonusProjectileLifespan
    }

    public Stat statType;
    public float statValue;
}