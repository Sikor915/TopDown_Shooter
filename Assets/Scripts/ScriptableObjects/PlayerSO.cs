using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    [Header("References")]
    public CreatureSO creatureSO;

    [Header("Events")]
    public UnityEvent onHitEvent;
    public UnityEvent<float> tookDamageEvent;
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
    public float manouverIFrameDuration;
    bool canBeHit = true;
    public bool CanBeHit
    {
        get { return canBeHit; }
        set { canBeHit = value; }
    }

    void OnEnable()
    {
        onHitEvent ??= new UnityEvent();
        tookDamageEvent ??= new UnityEvent<float>();
        canBeHit = true;
    }

    public void OnHit(float damage)
    {
        if (canBeHit)
        {
            onHitEvent?.Invoke();
            tookDamageEvent?.Invoke(damage);
            canBeHit = false;
        }
    }

}
