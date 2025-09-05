using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IPlayer
{
    [SerializeField] PlayerSO playerSO;
    [SerializeField] PlayerInventorySO playerInventorySO;

    Rigidbody2D rb2d;
    Vector2 m_GoalVel;
    Vector2 m_UnitGoal;

    Vector3 mousePos;

    bool isRolling = false;
    bool isSliding = false;

    Coroutine iFrameCoroutine;
    Coroutine rollCooldownCoroutine;
    Coroutine slideCooldownCoroutine;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<Weapon>(out var weaponC))
            {
                playerInventorySO.AddWeapon(child.gameObject);
                weaponC.pc = this;
                child.gameObject.SetActive(false);
            }
        }
        playerInventorySO.EquipWeapon(0); 
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerSO.CurrentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            playerInventorySO.EquipNextWeapon();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            playerInventorySO.EquipPreviousWeapon();
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

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (!context.performed || isRolling)
        {
            return;
        }
        playerSO.CanBeHit = false;
        StartIFrames();

        Vector2 targetVel = new(m_UnitGoal.x * playerSO.rollSpeed, m_UnitGoal.y * playerSO.rollSpeed);
        float t = playerSO.maxAccelForce * Time.fixedDeltaTime;
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
        StartIFrames();

        Vector2 targetVel = new(m_UnitGoal.x * playerSO.slideSpeed, m_UnitGoal.y * playerSO.slideSpeed);
        float t = playerSO.maxAccelForce * Time.fixedDeltaTime;
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

        Vector2 goalVel = m_UnitGoal * playerSO.maxSpeed;
        m_GoalVel = Vector2.MoveTowards(m_GoalVel,
        goalVel,
        playerSO.acceleration * Time.fixedDeltaTime);

        Vector2 neededAccel = (m_GoalVel - rb2d.linearVelocity) / Time.fixedDeltaTime;
        neededAccel = Vector2.ClampMagnitude(neededAccel, playerSO.maxAccelForce);
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
        yield return new WaitForSeconds(playerSO.iFrameDuration);
        playerSO.CanBeHit = true;
    }

    IEnumerator RollCooldown()
    {
        yield return new WaitForSeconds(playerSO.rollCooldown);
        isRolling = false;
    }

    IEnumerator SlideCooldown()
    {
        yield return new WaitForSeconds(playerSO.slideCooldown);
        isSliding = false;
    }

}
