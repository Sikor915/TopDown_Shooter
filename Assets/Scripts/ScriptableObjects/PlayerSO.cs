using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    [Header("References")]
    public CreatureSO creatureSO;

    [Header("Events")]
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
    bool canBeHit = true;
    public bool CanBeHit
    {
        get { return canBeHit; }
        set { canBeHit = value; }
    }

    void OnEnable()
    {
        onHitEvent ??= new UnityEvent();
        canBeHit = true;
    }

    public void OnHit(float damage)
    {
        if (canBeHit)
        {
            creatureSO.DeductHealth(damage);
            onHitEvent?.Invoke();
            canBeHit = false;
        }
    }

}
