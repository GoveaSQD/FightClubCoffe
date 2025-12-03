using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Jugador1 : MonoBehaviour
{
    // --- VARIABLES DE CONFIGURACIÓN (Públicas para el Inspector) ---
    public float baseSpeed = 5f;
    public int baseDano = 4;
    public float speedBoostAmount = 1.0f;
    public int danoBoostAmount = 2;
    public float buffDuration = 3f;
    public float stunTime = 1;
    private bool isKnockedback;

    // --- REFERENCIAS Y ESTADO ---
    private JugadorResoursesCollector collector;
    private Rigidbody2D rb2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer; // <-- NUEVO: para flip controlado

    private Vector2 movementInput;
    private bool gameIsPaused = false;

    private int currentDano;
    private float currentSpeed;

    public Player_Combat player_Combat;
    void Awake()
    {
        Jugador1[] players = Object.FindObjectsByType<Jugador1>(FindObjectsSortMode.None);

        if (players.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // <-- asignación

        collector = GetComponent<JugadorResoursesCollector>();

        currentSpeed = baseSpeed;
        currentDano = baseDano;

        ConsumoCafe.Instance.UpdateDano(currentDano);
        ConsumoCafe.Instance.UpdateVelocidad((int)currentSpeed);
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
        GameObject spawn = GameObject.Find("PlayerSpawn");
        if (spawn != null)
        {
            transform.position = spawn.transform.position;
        }
    }


    public void ResetStatsToBase()
    {
        currentSpeed = baseSpeed;
        currentDano = baseDano;

        ConsumoCafe.Instance.UpdateDano(currentDano);
        ConsumoCafe.Instance.UpdateVelocidad((int)currentSpeed);
    }


    void Update()
    {
        if (Input.GetButtonDown("Slash"))
        {
            // Pasamos la llamada tal cual al Player_Combat (usa Input dentro si quieres)
            player_Combat.Attack();
        }

        if (gameIsPaused)
        {
            animator.SetFloat("Speed", 0);
        }

        if (!isKnockedback)
        {
            // Leer input de movimiento (4 direcciones)
            movementInput.x = Input.GetAxisRaw("Horizontal");
            movementInput.y = Input.GetAxisRaw("Vertical");
            movementInput = movementInput.normalized;

            // Actualizar parámetros de animator para caminar (no tocamos AttackX aquí)
            animator.SetFloat("Horizontal", movementInput.x);
            animator.SetFloat("Vertical", movementInput.y);
            animator.SetFloat("Speed", movementInput.magnitude);
        }

        // Actualizar attackPoint solo cuando NO estás atacando (mantienes tu lógica)
        if (!player_Combat.isAttacking && movementInput != Vector2.zero)
        {
            player_Combat.attackPoint.localPosition = new Vector3(
                Mathf.Round(movementInput.x),
                Mathf.Round(movementInput.y),
                0
            );
        }

        // Detener movimiento mientras atacas o estás knockedback
        if (!player_Combat.isAttacking && !isKnockedback)
        {
            // Ya actualizamos movementInput arriba, aseguramos solo eje X para rb en FixedUpdate
            // (mantener lógica)
        }
        else
        {
            movementInput = Vector2.zero;
        }

        // --- Nuevo: flip controlado SOLO si hay movimiento horizontal visible ---
        // Esto evita que al moverte verticalmente el sprite se vea volteado.
        if (spriteRenderer != null)
        {
            // Si hay movimiento horizontal suficiente, aplicamos flip según su signo
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f)
            {
                spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") < 0f;
            }
            // Si no hay movimiento horizontal (ej. estás yendo arriba/abajo),
            // dejamos el flip tal como está (no lo cambiamos), evitando que mire a la izquierda por accidente.
        }

        OpenCloseInventory();
        OpenClosePauseMenu();
        CheckConsumoInput();
    }


     private void FixedUpdate()
    {
        if (!gameIsPaused)
        {
            // CORRECCIÓN: usar 'velocity' en lugar de 'linearVelocity'
            rb2D.linearVelocity = movementInput * currentSpeed;
        }
        else
        {
            rb2D.linearVelocity = Vector2.zero;
        }
    }

    // --- MÉTODOS DE UI Y PAUSA ---

    void OpenCloseInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            UIManager.Instance.OpenCloseInventory();
        }
    }

    void OpenClosePauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (gameIsPaused)
            {
                UIManager.Instance.ResumeGame();
                gameIsPaused = false;
            }
            else
            {
                UIManager.Instance.PauseGame();
                gameIsPaused = true; 
            } 
        } 
    }

    // --- MÉTODOS DE CONSUMO ---

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
            // 1. Reducir el inventario (Actualiza el contador del inventario)
            collector.DecreaseItemCount(itemTag);
            
            // 2. Detener y Reiniciar el Buff (para que cada consumo refresque el temporizador)
            StopCoroutine("ApplyBuff"); 
            StartCoroutine("ApplyBuff"); 
            
            Debug.Log($"Consumiste {itemTag}. Iniciando buff de {buffDuration} segundos.");
        }
        else
        {
            Debug.Log($"No tienes {itemTag} para consumir o el Collector es nulo.");
        }
    }

    // --- CORRUTINA DE EFECTO TEMPORAL ---

    IEnumerator ApplyBuff()
    {
        // --- 1. APLICAR BUFF ---
        
        // Aplicar el aumento de daño y velocidad
        currentDano += danoBoostAmount;
        currentSpeed += speedBoostAmount;
        
        // Actualizar UI
        ConsumoCafe.Instance.UpdateDano(currentDano);
        ConsumoCafe.Instance.UpdateVelocidad((int)currentSpeed);
        
        Debug.Log("Buff APLICADO: +Daño y +Velocidad.");

        // --- 2. ESPERAR ---
        
        // La ejecución se detiene aquí por la duración
        yield return new WaitForSeconds(buffDuration);

        // --- 3. QUITAR BUFF ---
        
        // Revertir el daño y la velocidad
        currentDano -= danoBoostAmount;
        currentSpeed -= speedBoostAmount;
        
        // Actualizar UI
        ConsumoCafe.Instance.UpdateDano(currentDano);
        ConsumoCafe.Instance.UpdateVelocidad((int)currentSpeed);

        Debug.Log("Buff TERMINADO: Daño y Velocidad restaurados.");
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockedback = true;
        Vector2 direction = (transform.position - enemy.position).normalized * force;
        rb2D.linearVelocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));
    }

    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb2D.linearVelocity = Vector2.zero;
        isKnockedback = false;
    }
}