using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Jugador1 : MonoBehaviour
{
    // ---------------- CONFIG / STATS ----------------
    [Header("Movimiento")]
    public float baseSpeed = 5f;
    [HideInInspector] public float currentSpeed;

    [Header("Daño / Buffs")]
    public int baseDano = 4;
    [HideInInspector] public int currentDano;
    public float speedBoostAmount = 1f;
    public int danoBoostAmount = 2;
    public float buffDuration = 3f;

    [Header("Knockback")]
    public float defaultStunTime = 1f;
    private bool isKnockedback = false;

    // ---------------- REFERENCIAS ----------------
    [Header("Referencias (asignar en Inspector si quieres)")]
    public Rigidbody2D rb2D;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Player_Combat player_Combat;
    public JugadorResoursesCollector collector; // opcional

    // ---------------- RUNTIME STATE ----------------
    [HideInInspector] public bool isFacingRight = true;
    [HideInInspector] public bool isMoving = false;

    private Vector2 movementInput;
    private bool gameIsPaused = false;

    // ---------------- PERSISTENCIA / SINGLETON ----------------
    private static Jugador1 instance;

    // ---------------- UNITY LIFECYCLE ----------------
    void Awake()
    {
        // Evitar duplicados al usar DontDestroyOnLoad
        Jugador1[] players = FindObjectsOfType<Jugador1>();
        if (players.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Intentar autoconfigurar componentes si no fueron asignados
        if (rb2D == null) rb2D = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (player_Combat == null) player_Combat = GetComponent<Player_Combat>();
        if (collector == null) collector = GetComponent<JugadorResoursesCollector>();
    }

    void Start()
    {
        // Inicializar stats
        currentSpeed = baseSpeed;
        currentDano = baseDano;

        // Actualizar UI si existe
        if (ConsumoCafe.Instance != null)
        {
            ConsumoCafe.Instance.UpdateDano(currentDano);
            ConsumoCafe.Instance.UpdateVelocidad((int)currentSpeed);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reposicionar en spawn si existe
        GameObject spawn = GameObject.Find("PlayerSpawn");
        if (spawn != null)
        {
            transform.position = spawn.transform.position;
        }
    }

    void Update()
    {
        // Input de ataque: permitir que Player_Combat gestione su cooldown internamente
        if (Input.GetButtonDown("Slash") || Input.GetKeyDown(KeyCode.Space))
        {
            // Dejar que Player_Combat gestione condiciones (ej. cooldown)
            if (player_Combat != null)
                player_Combat.Attack();
        }

        // Atajos UI / pausa / inventario
        OpenCloseInventory();
        OpenClosePauseMenu();
        CheckConsumoInput();

        // Pausa visual
        if (gameIsPaused)
        {
            if (animator != null) animator.SetFloat("Speed", 0f);
            return;
        }

        // Movimiento: leer input salvo esté en knockback
        if (!isKnockedback)
        {
            movementInput.x = Input.GetAxisRaw("Horizontal");
            movementInput.y = Input.GetAxisRaw("Vertical");
            movementInput = movementInput.normalized;
        }
        else
        {
            // si hay knockback, anular input de movimiento (solo knockback debe mover)
            movementInput = Vector2.zero;
        }

        // Actualizar bandera isMoving
        isMoving = movementInput.magnitude > 0.1f;

        // Volteo: permitir flip solo si no hay knockback
        if (!isKnockedback)
        {
            if (movementInput.x > 0.1f && !isFacingRight)
                Flip();
            else if (movementInput.x < -0.1f && isFacingRight)
                Flip();
        }

        // Animaciones: setear parámetros (mantener compatibilidad con tu animator)
        if (animator != null)
        {
            animator.SetFloat("Horizontal", movementInput.x);
            animator.SetFloat("Vertical", movementInput.y);
            animator.SetFloat("Speed", movementInput.magnitude);
        }
    }

    void FixedUpdate()
    {
        if (!gameIsPaused)
        {
            if (!isKnockedback)
            {
                // Aplicar movimiento usando currentSpeed
                rb2D.linearVelocity = movementInput * currentSpeed;
            }
            // si está en knockback, no sobreescribimos la velocidad aquí
        }
        else
        {
            rb2D.linearVelocity = Vector2.zero;
        }
    }

    // ---------------- HELPERS ----------------

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        if (spriteRenderer != null)
            spriteRenderer.flipX = !isFacingRight;
    }

    // ---------------- CONSUMO / BUFF ----------------

    void CheckConsumoInput()
    {
        if (Input.GetKeyDown(KeyCode.Q)) TryConsumeItem("CafeBlanco");
        if (Input.GetKeyDown(KeyCode.W)) TryConsumeItem("Capuchino");
        if (Input.GetKeyDown(KeyCode.E)) TryConsumeItem("CafeNegro");
        if (Input.GetKeyDown(KeyCode.R)) TryConsumeItem("B52");
    }

    void TryConsumeItem(string itemTag)
    {
        if (collector != null && collector.GetItemCount(itemTag) > 0)
        {
            collector.DecreaseItemCount(itemTag);
            StopCoroutine(ApplyBuffCoroutineName);
            StartCoroutine(ApplyBuff());
        }
        else
        {
            Debug.Log($"No tienes {itemTag} o collector es nulo.");
        }
    }

    const string ApplyBuffCoroutineName = "ApplyBuff";
    IEnumerator ApplyBuff()
    {
        currentDano += danoBoostAmount;
        currentSpeed += speedBoostAmount;

        if (ConsumoCafe.Instance != null)
        {
            ConsumoCafe.Instance.UpdateDano(currentDano);
            ConsumoCafe.Instance.UpdateVelocidad((int)currentSpeed);
        }

        yield return new WaitForSeconds(buffDuration);

        currentDano -= danoBoostAmount;
        currentSpeed -= speedBoostAmount;

        if (ConsumoCafe.Instance != null)
        {
            ConsumoCafe.Instance.UpdateDano(currentDano);
            ConsumoCafe.Instance.UpdateVelocidad((int)currentSpeed);
        }
    }

    // ---------------- KNOCKBACK ----------------

    /// <summary>
    /// Empuja al jugador y evita que controle durante stunTime segundos.
    /// </summary>
    public void Knockback(Transform enemy, float force, float stunTime)
    {
        // cancelar animación de ataque para evitar que se quede atascada
        if (animator != null) animator.SetBool("isAttacking", false);

        isKnockedback = true;

        Vector2 dir = (transform.position - enemy.position).normalized;
        rb2D.linearVelocity = dir * force;

        // iniciar fin del knockback
        StopCoroutine(KnockbackCoroutineName);
        StartCoroutine(KnockbackCoroutine(stunTime));
    }

    const string KnockbackCoroutineName = "KnockbackCoroutine";
    IEnumerator KnockbackCoroutine(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb2D.linearVelocity = Vector2.zero;
        isKnockedback = false;

        // asegurarse que la animación de ataque está en false
        if (animator != null) animator.SetBool("isAttacking", false);
    }

    // ---------------- UI / PAUSA / INVENTARIO ----------------
    void OpenCloseInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (UIManager.Instance != null) UIManager.Instance.OpenCloseInventory();
        }
    }

    void OpenClosePauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (gameIsPaused)
            {
                if (UIManager.Instance != null) UIManager.Instance.ResumeGame();
                gameIsPaused = false;
            }
            else
            {
                if (UIManager.Instance != null) UIManager.Instance.PauseGame();
                gameIsPaused = true;
            }
        }
    }

    // ---------------- UTIL ----------------
    public int GetCurrentDano()
    {
        return currentDano;
    }

    // Para reiniciar stats si comienzas partida nueva
    public void ResetStatsToBase()
    {
        currentSpeed = baseSpeed;
        currentDano = baseDano;

        if (ConsumoCafe.Instance != null)
        {
            ConsumoCafe.Instance.UpdateDano(currentDano);
            ConsumoCafe.Instance.UpdateVelocidad((int)currentSpeed);
        }
    }

    // ---------------- GIZMOS (opcional) ----------------
    void OnDrawGizmosSelected()
    {
        // Useful for debugging movement/hitboxes in scene if needed
    }
}
