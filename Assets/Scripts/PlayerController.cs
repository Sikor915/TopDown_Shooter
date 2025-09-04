using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IPlayer
{
    [SerializeField] PlayerSO playerSO;
    [SerializeField] float maxSpeed = 10.0f;
    [SerializeField] float acceleration = 50.0f;
    [SerializeField] float maxAccelForce = 150.0f;
    [SerializeField] float rollSpeed = 30.0f;
    [SerializeField] float slideSpeed = 70.0f;

    Rigidbody2D rb2d;
    Vector2 m_GoalVel;
    Vector2 m_UnitGoal;

    Vector3 mousePos;

    bool isShooting = false;
    bool isRolling = false;
    bool isSliding = false;
    readonly float rollCooldown = 1.0f;
    readonly float slideCooldown = 1.5f;
    readonly float iFrameDuration = 1.0f;

    Weapon weapon;

    Coroutine iFrameCoroutine;
    Coroutine rollCooldownCoroutine;
    Coroutine slideCooldownCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        weapon = GetComponentInChildren<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerSO.CurrentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
        if (Input.GetMouseButton(0))
        {
            OnAttack();
        }
    }

    void OnEnable()
    {
        playerSO.onHitEvent.AddListener(StartIFrames);
    }

    private void FixedUpdate()
    {
        RotateToCursor();
        MoveCharacter();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        m_UnitGoal = context.ReadValue<Vector2>();
    }

    public void OnAttack()
    {
        weapon.PrimaryAction();
        weapon.HandleShoot();
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            weapon.TryReload();
        }
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (!context.performed || isRolling)
        {
            return;
        }
        playerSO.CanBeHit = false;
        if (iFrameCoroutine != null)
        {
            StopCoroutine(iFrameCoroutine);
        }
        iFrameCoroutine = StartCoroutine(IFrames());

        Vector2 targetVel = new(m_UnitGoal.x * rollSpeed, m_UnitGoal.y * rollSpeed);
        float t = maxAccelForce * Time.fixedDeltaTime;
        rb2d.linearVelocity = Vector2.Lerp(rb2d.linearVelocity, targetVel, t);

        isRolling = true;
        if (rollCooldownCoroutine != null)
        {
            StopCoroutine(rollCooldownCoroutine);
        }
        rollCooldownCoroutine = StartCoroutine(RollCooldown());

    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        if (!context.performed || isSliding)
        {
            return;
        }
        playerSO.CanBeHit = false;
        if (iFrameCoroutine != null)
        {
            StopCoroutine(iFrameCoroutine);
        }
        iFrameCoroutine = StartCoroutine(IFrames());

        Vector2 targetVel = new(m_UnitGoal.x * slideSpeed, m_UnitGoal.y * slideSpeed);
        float t = maxAccelForce * Time.fixedDeltaTime;
        rb2d.linearVelocity = Vector2.Lerp(rb2d.linearVelocity, targetVel, t);

        isSliding = true;
        if (slideCooldownCoroutine != null)
        {
            StopCoroutine(slideCooldownCoroutine);
        }
        slideCooldownCoroutine = StartCoroutine(SlideCooldown());

    }

    void RotateToCursor()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 perpendicular = Vector3.Cross(transform.position - mousePos, Vector3.forward);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);
    }

    void MoveCharacter()
    {
        //This here can be later used to improve acceleration when switching directions
        //Vector2 unitVel = m_GoalVel.normalized;
        //float velDot = Vector2.Dot(m_UnitGoal, unitVel);

        Vector2 goalVel = m_UnitGoal * maxSpeed;
        m_GoalVel = Vector2.MoveTowards(m_GoalVel,
        goalVel,
        acceleration * Time.fixedDeltaTime);

        Vector2 neededAccel = (m_GoalVel - rb2d.linearVelocity) / Time.fixedDeltaTime;
        neededAccel = Vector2.ClampMagnitude(neededAccel, maxAccelForce);
        rb2d.AddForce(neededAccel, ForceMode2D.Force);
    }

    void StartIFrames()
    {
        if (iFrameCoroutine != null)
        {
            StopCoroutine(iFrameCoroutine);
        }
        iFrameCoroutine = StartCoroutine(IFrames());
    }

    IEnumerator IFrames()
    {
        yield return new WaitForSeconds(iFrameDuration);
        playerSO.CanBeHit = true;
    }

    IEnumerator RollCooldown()
    {
        yield return new WaitForSeconds(rollCooldown);
        isRolling = false;
    }

    IEnumerator SlideCooldown()
    {
        yield return new WaitForSeconds(slideCooldown);
        isSliding = false;
    }

}
