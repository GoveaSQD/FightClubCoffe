using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Jugador1 : MonoBehaviour
{
    // --- VARIABLES DE CONFIGURACIÓN (Públicas para el Inspector) ---
    public float baseSpeed = 5f;         // Velocidad inicial base
    public int baseDano = 4;            // Daño inicial base
    public float speedBoostAmount = 1.0f;  // Aumento de velocidad al consumir
    public int danoBoostAmount = 2;      // Aumento de daño al consumir
    public float buffDuration = 3f;      // Duración del efecto en segundos
    public float stunTime = 1;        // Duración del aturdimiento por knockback
    private bool isKnockedback;

    // --- REFERENCIAS Y ESTADO ---
    private JugadorResoursesCollector collector; // Referencia al script de recolección
    private Rigidbody2D rb2D;
    private Animator animator;
    
    private Vector2 movementInput;
    private bool gameIsPaused = false;
    
    private int currentDano;    // Daño total actual (base + buff)
    private float currentSpeed; // Velocidad total actual (base + buff)

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
        
        // **IMPORTANTE**: Asegúrate de que el script JugadorCollector esté en este mismo GameObject.
        collector = GetComponent<JugadorResoursesCollector>(); 
        
        
        // Inicialización de efectos al estado base
        currentSpeed = baseSpeed;
        currentDano = baseDano;
        
        // Actualizar UI inicial (Asumiendo que ConsumoCafe.Instance ya está inicializado)        
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
            player_Combat.Attack();
        }
        if (gameIsPaused) 
        {
            // Opcional: Detener la animación si está pausado
            animator.SetFloat("Speed", 0); 
        }
        if (isKnockedback == false)
        {
        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        movementInput = movementInput.normalized;

        animator.SetFloat("Horizontal", movementInput.x);
        animator.SetFloat("Vertical", movementInput.y);
        // Usa la magnitud del input para la animación, incluso si la velocidad es 0 (pausa)
        animator.SetFloat("Speed", movementInput.magnitude); 
        }    
        OpenCloseInventory();
        OpenClosePauseMenu();
        CheckConsumoInput(); 
    }

    private void FixedUpdate()
    { 
        if (!gameIsPaused) 
        {
            // Usa currentSpeed para aplicar el movimiento
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