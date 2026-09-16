using UnityEngine;
using System.Collections;

public enum ShipState
{
    Normal,
    Dashing,
    Disabled
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class TopDownShipController : MonoBehaviour
{
    [Header("Movement Stats")]
    public float acceleration = 20f;
    public float maxSpeed = 6.67f;
    public float rotationSpeed = 300f;
    [Range(0f, 1f)] public float strafeFactor = 0.7f;
    [Range(0f, 1f)] public float backwardFactor = 0.5f;

    [Header("Dash Settings")]
    [Tooltip("Общая длительность рывка в секундах. НЕ СТАВЬТЕ 0!")]
    public float dashBaseDuration = 0.5f;  
    public float dashCooldown = 1.0f;       
    public float dashDistanceMulti = 2.0f; 

    [Header("Refs")]
    public Bullet bulletPrefab;
    public Transform firePoint;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private ShipState currentState = ShipState.Normal;
    private bool isDashRamming = false; 
    private float nextDashTime = 0f;
    
    private Vector2 movementInput;
    private float currentHealth;
    public float maxHealth = 6f;

    public static TopDownShipController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (dashBaseDuration <= 0.1f) dashBaseDuration = 0.5f;
        if (dashDistanceMulti <= 0f) dashDistanceMulti = 2f;
    }

    private void Start()
    {
        rb.gravityScale = 0f;
        rb.linearDamping = 1f;
        currentHealth = maxHealth;
        GameEvents.HealthChanged(currentHealth, maxHealth);
    }

    private void Update()
    {
        if (currentState != ShipState.Normal) return;

        ProcessInput();

        bool canDash = Time.time >= nextDashTime;
        
        bool isUnlocked = true;
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (canDash && isUnlocked)
            {
                Debug.Log("Dash Started!");
                StartCoroutine(DashRoutine());
            }
            else
            {
                Debug.Log($"Dash failed. Cooldown: {!canDash}, Unlocked: {isUnlocked}");
            }
        }

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            Shoot();
    }

    private void FixedUpdate()
    {
        if (currentState == ShipState.Normal)
        {
            MoveShip();
            RotateShip();
            ClampVelocity();
        }
    }

    private IEnumerator DashRoutine()
    {
        currentState = ShipState.Dashing;
        nextDashTime = Time.time + dashBaseDuration + dashCooldown;

        float tStart = dashBaseDuration * 0.2f;
        float tActive = dashBaseDuration * 0.7f;
        float tRecover = dashBaseDuration * 0.1f;

        Vector2 dashDir = transform.up; // Куда смотрит нос

        float spriteHeight = (spriteRenderer.sprite != null) ? spriteRenderer.sprite.bounds.size.y : 1f;
        float shipSize = spriteHeight * transform.localScale.y;
        
        float safeActiveTime = Mathf.Max(tActive, 0.01f);
        float dashSpeed = (shipSize * dashDistanceMulti) / safeActiveTime;

        Debug.Log($"Dash Physics: Dist={shipSize * dashDistanceMulti}, Speed={dashSpeed}");

        // startup
        rb.linearVelocity *= 0.5f; 
        yield return new WaitForSeconds(tStart);

        // active: только здесь работает таран
        isDashRamming = true;
        
        float startTime = Time.time;
        float endTime = startTime + safeActiveTime;

        while (Time.time < endTime)
        {
            rb.linearVelocity = dashDir * dashSpeed;
            // Без yield цикл вешает редактор.
            yield return new WaitForFixedUpdate(); 
        }

        // recovery
        isDashRamming = false;
        rb.linearVelocity *= 0.1f;
        yield return new WaitForSeconds(tRecover);

        currentState = ShipState.Normal;
        Debug.Log("Dash Finished");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashRamming && collision.gameObject.CompareTag("Asteroid"))
        {
            Debug.Log("Ramming Asteroid!");
            GameEvents.AsteroidDestroyed(collision.transform.position);
            Destroy(collision.gameObject);
            return;
        }

        if (!isDashRamming && currentState == ShipState.Normal && collision.gameObject.CompareTag("Asteroid"))
        {
             Asteroid asteroid = collision.gameObject.GetComponent<Asteroid>();
             DamageType dt = asteroid != null ? asteroid.GetDamageType() : DamageType.Half;
             TakeDamage(dt);
        }
    }
    
    private void ProcessInput()
    {
        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private void MoveShip()
    {
        if (movementInput.sqrMagnitude < 0.01f) return;
        Vector2 force = Vector2.zero;
        if (movementInput.y > 0) force.y = movementInput.y;
        else force.y = movementInput.y * backwardFactor;
        force.x = movementInput.x * strafeFactor;
        rb.AddRelativeForce(force * acceleration);
    }

    private void RotateShip()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        rb.MoveRotation(Mathf.MoveTowardsAngle(rb.rotation, angle, rotationSpeed * Time.fixedDeltaTime));
    }
    
    private void ClampVelocity()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
             rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }

    private void Shoot()
    {
        if(bulletPrefab != null) {
            Bullet b = Instantiate(bulletPrefab, firePoint != null ? firePoint.position : transform.position, transform.rotation);
            b.Project(transform.up);
        }
    }

    public void TakeDamage(DamageType type)
    {
        if (isDashRamming) return;

        float dmg = type == DamageType.Half ? 1f : 2f;
        currentHealth -= dmg;
        GameEvents.HealthChanged(currentHealth, maxHealth);
        GameEvents.PlayerTookDamage(type);

        if (currentHealth <= 0)
        {
            rb.linearVelocity = Vector2.zero;
            GameEvents.PlayerDied();
            GameEvents.GameOver();
            gameObject.SetActive(false);
        }
    }

    public void RestoreFullHealth()
    {
        currentHealth = maxHealth;
        GameEvents.HealthChanged(currentHealth, maxHealth);
    }
    
    public void ResetShip()
    {
        if(rb==null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        transform.rotation = Quaternion.identity;
        currentState = ShipState.Normal; 
        isDashRamming = false;
        RestoreFullHealth();
        gameObject.SetActive(true);
    }
    
    public void SetImmune(bool immune, float duration) {}
}