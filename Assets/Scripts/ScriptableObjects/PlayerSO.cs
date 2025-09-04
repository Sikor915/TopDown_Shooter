using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public UnityEvent<float, float> onHealthChangedEvent;
    public UnityEvent onHitEvent;


    float maxHealth = 100;
    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; onHealthChangedEvent?.Invoke(currentHealth, maxHealth); }
    }
    float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; onHealthChangedEvent?.Invoke(currentHealth, maxHealth); }
    }

    bool canBeHit = true;
    public bool CanBeHit
    {
        get { return canBeHit; }
        set { canBeHit = value; }
    }

    void OnEnable()
    {
        onHealthChangedEvent ??= new UnityEvent<float, float>();
        onHitEvent ??= new UnityEvent();
        currentHealth = maxHealth;
        canBeHit = true;
    }

    public void Reset()
    {
        currentHealth = maxHealth;
    }

    public void OnHit(int damage)
    {
        Debug.Log("PlayerSO OnHit called. canBeHit: " + canBeHit);
        if (canBeHit)
        {
            Debug.Log("PlayerSO DeductHealth called");
            DeductHealth(damage);
            onHitEvent?.Invoke();
            canBeHit = false;
        }
    }

    public void DeductHealth(int damage)
    {
        CurrentHealth -= damage;
        Debug.Log("Player hit for " + damage + " damage. Current health: " + currentHealth);
        onHealthChangedEvent?.Invoke(currentHealth, maxHealth);
    }

}
