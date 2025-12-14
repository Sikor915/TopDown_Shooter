using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(PlayerInventory))]
public class PlayerController : Singleton<PlayerController>, IPlayer
{
    [SerializeField] PlayerSO playerSO;
    public PlayerSO PlayerSO => playerSO;
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] AudioClip playerHurtSound;

    float currentHealth;
    public float CurrentHealth => currentHealth;

    Rigidbody2D rb2d;
    Vector2 m_GoalVel;
    Vector2 m_UnitGoal;

    AudioMixerGroup audioMixerGroup;


    bool isRolling = false;
    bool rollingCooldown = false;
    bool isSliding = false;
    bool slidingCooldown = false;

    Coroutine iFrameCoroutine;
    Coroutine manouverIFrameCoroutine;
    Coroutine rollCooldownCoroutine;
    Coroutine slideCooldownCoroutine;

    void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        audioMixerGroup = GetComponent<AudioSource>().outputAudioMixerGroup;
        DontDestroyOnLoad(gameObject);
        currentHealth = playerSO.creatureSO.MaxHealth;
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
        playerInventory.EquipWeapon(1);
        playerInventory.EquipWeapon(0);
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

        if (Input.GetKeyDown(KeyCode.E) && PlayerInteractManager.Instance.IsPlayerNearInteractable())
        {
            UIController.Instance.RunPlayerInteractProgressBar();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            UIController.Instance.StopPlayerInteractProgressBar();
        }
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            UIController.Instance.ToggleSettingsMenu();
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
        if (!context.performed || isRolling || rollingCooldown)
        {
            return;
        }
        playerSO.CanBeHit = false;
        StartManouverIFrames();

        Vector2 targetVel = new(m_UnitGoal.x * playerSO.rollSpeed, m_UnitGoal.y * playerSO.rollSpeed);
        float t = playerSO.maxAccelForce * Time.fixedDeltaTime;
        rb2d.linearVelocity = Vector2.Lerp(rb2d.linearVelocity, targetVel, t);

        if (isSliding)
        {
            isSliding = false;
            PlayerAnimationController.Instance.SetIsSliding(false);
        }

        isRolling = true;
        rollingCooldown = true;
        if (rollCooldownCoroutine != null)
        {
            StopCoroutine(rollCooldownCoroutine);
        }
        rollCooldownCoroutine = StartCoroutine(RollCooldown());
        PlayerAnimationController.Instance.SetIsRolling(true);
    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        if (!context.performed || isSliding || slidingCooldown)
        {
            return;
        }
        playerSO.CanBeHit = false;
        StartManouverIFrames();

        Vector2 targetVel = new(m_UnitGoal.x * playerSO.slideSpeed, m_UnitGoal.y * playerSO.slideSpeed);
        float t = playerSO.maxAccelForce * Time.fixedDeltaTime;
        rb2d.linearVelocity = Vector2.Lerp(rb2d.linearVelocity, targetVel, t);

        if (isRolling)
        {
            isRolling = false;
            PlayerAnimationController.Instance.SetIsRolling(false);
        }

        isSliding = true;
        slidingCooldown = true;
        if (slideCooldownCoroutine != null)
        {
            StopCoroutine(slideCooldownCoroutine);
        }
        slideCooldownCoroutine = StartCoroutine(SlideCooldown());
        PlayerAnimationController.Instance.SetIsSliding(true);
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        Debug.Log("Menu button pressed");
    }

    public void RestoreHealth(float amount)
    {
        Debug.Log("Restoring Health: " + amount);
        currentHealth = Mathf.Min(currentHealth + amount, playerSO.creatureSO.MaxHealth);
        playerSO.creatureSO.onHealthChangedEvent?.Invoke(currentHealth, playerSO.creatureSO.MaxHealth);
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
        bool value = audioMixerGroup.audioMixer.GetFloat("SFXVolume", out float volume);
        volume = Mathf.Pow(10, volume / 20);
        AudioSource.PlayClipAtPoint(playerHurtSound, transform.position, volume);
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

        PlayerAnimationController.Instance.SetIsMoving(m_UnitGoal.magnitude > 0.1f && !isRolling && !isSliding);

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

    void StartManouverIFrames()
    {
        if (manouverIFrameCoroutine != null)
        {
            StopCoroutine(manouverIFrameCoroutine);
        }
        manouverIFrameCoroutine = StartCoroutine(ManouverIFrames());
    }

    IEnumerator ManouverIFrames()
    {
        yield return new WaitForSeconds(playerSO.manouverIFrameDuration);
        playerSO.CanBeHit = true; if (isRolling)
        {
            isRolling = false;
            PlayerAnimationController.Instance.SetIsRolling(false);
        }
        if (isSliding)
        {
            isSliding = false;
            PlayerAnimationController.Instance.SetIsSliding(false);
        }
    }

    IEnumerator IFrames()
    {
        PlayerAnimationController.Instance.FlashRed();
        yield return new WaitForSeconds(playerSO.iFrameDuration);
        playerSO.CanBeHit = true;

    }

    IEnumerator RollCooldown()
    {
        yield return new WaitForSeconds(playerSO.rollCooldown);
        rollingCooldown = false;
    }

    IEnumerator SlideCooldown()
    {
        yield return new WaitForSeconds(playerSO.slideCooldown);
        slidingCooldown = false;
    }

}
