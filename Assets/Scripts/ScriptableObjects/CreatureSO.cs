using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "CreatureSO", menuName = "Scriptable Objects/CreatureSO")]
public class CreatureSO : ScriptableObject
{
    [Header("Events")]
    public UnityEvent<float, float> onHealthChangedEvent;
    public UnityEvent onStatsChangedEvent;

    [Header("Stats/HP")]
    [SerializeField] float maxHealth = 100;
    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; onHealthChangedEvent?.Invoke(currentHealth, maxHealth); }
    }
    [SerializeField] float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; onHealthChangedEvent?.Invoke(currentHealth, maxHealth); }
    }

    [Header("Stats/Damage")]
    public float bonusDamage;
    public float percentBonusDamage;
    public float percentCritChance;
    public float percentCritDamage;

    [Header("Stats/Guns")]
    public float percentBonusReloadSpeed;
    public float percentBonusFireRate;
    public float percentBonusProjectileSpeed;
    public float percentBonusProjectileLifespan;

    [Header("Base Stats")]
    [SerializeField] float baseBonusDamage;
    [SerializeField] float basePercentBonusDamage;
    [SerializeField] float basePercentCritChance;
    [SerializeField] float basePercentCritDamage;
    [SerializeField] float basePercentBonusReloadSpeed;
    [SerializeField] float basePercentBonusFireRate;
    [SerializeField] float basePercentBonusProjectileSpeed;
    [SerializeField] float basePercentBonusProjectileLifespan;
    void OnEnable()
    {

        onHealthChangedEvent ??= new UnityEvent<float, float>();
        currentHealth = maxHealth;
    }

    public void Reset()
    {
        currentHealth = maxHealth;
    }

    public void DeductHealth(int damage)
    {
        CurrentHealth -= damage;
        onHealthChangedEvent?.Invoke(currentHealth, maxHealth);
    }
}
