using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    [Header("Events")]
    public UnityEvent<float, float> onHealthChangedEvent;
    public UnityEvent onHitEvent;
    [Header("Movement Stats")]
    public float maxSpeed = 25.0f;
    public float acceleration = 200.0f;
    public float maxAccelForce = 150.0f;

    [Header("Evasive Movement Stats")]
    public float rollSpeed;
    public float slideSpeed;
    public float rollCooldown;
    public float slideCooldown;
    public float iFrameDuration;

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
        if (canBeHit)
        {
            DeductHealth(damage);
            onHitEvent?.Invoke();
            canBeHit = false;
        }
    }

    public void DeductHealth(int damage)
    {
        CurrentHealth -= damage;
        onHealthChangedEvent?.Invoke(currentHealth, maxHealth);
    }

}
