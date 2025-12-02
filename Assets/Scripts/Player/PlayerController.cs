using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(PlayerInventory))]
public class PlayerController : Singleton<PlayerController>, IPlayer
{
    [SerializeField] PlayerSO playerSO;
    public PlayerSO PlayerSO => playerSO;
    [SerializeField] PlayerInventory playerInventory;

    float currentHealth;
    public float CurrentHealth => currentHealth;

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
        DontDestroyOnLoad(gameObject);
        rb2d = GetComponent<Rigidbody2D>();
        foreach (Transform child in transform.GetChild(0))
        {
            if (child.TryGetComponent<Weapon>(out var weaponC))
            {
                playerInventory.AddWeapon(child.gameObject);
                child.gameObject.SetActive(false);
                weaponC.ownerCreatureSO = playerSO.creatureSO;
            }
        }
        playerInventory.EquipWeapon(0); 
        currentHealth = playerSO.creatureSO.MaxHealth;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerSO.creatureSO.onHealthChangedEvent?.Invoke(currentHealth, playerSO.creatureSO.MaxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
            UIController.Instance.PlayerDead();
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            playerInventory.EquipNextWeapon();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            playerInventory.EquipPreviousWeapon();
        }

        if (Input.GetMouseButtonDown(1))
        {
            PlayerAim.Instance.ThrowWeapon();
        }
    }

    void OnEnable()
    {
        playerSO.onHitEvent.AddListener(StartIFrames);
        playerSO.tookDamageEvent.AddListener(TakeDamage);
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        PlayerInteractManager.Instance.ProcessInteraction();
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (playerInventory.currentWeapon != null)
        {
            Vector3 dropPosition = transform.position + transform.up * 1.0f; // Drop in front of the player
            playerInventory.DropWeapon(playerInventory.currentWeapon, dropPosition);
        }
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

    void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Weapon"))
        {
            PlayerInteractManager.Instance.NearestWeapon = coll.gameObject;
            PlayerInteractManager.Instance.CanPickUp = true;
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Weapon"))
        {
            PlayerInteractManager.Instance.NearestWeapon = null;
            PlayerInteractManager.Instance.CanPickUp = false;
        }
    }

    void TakeDamage(float damage)
    {
        currentHealth -= damage;
        playerSO.creatureSO.onHealthChangedEvent?.Invoke(currentHealth, playerSO.creatureSO.MaxHealth);
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
