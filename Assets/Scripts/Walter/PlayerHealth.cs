using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public GameObject gameOverPanel;
    
    public Vector3 initialPosition;

    public void Awake()
    {
        // **CORRECCIÓN CRÍTICA**: Convertir en GameObject raíz
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        
        // Suscribirse al evento de carga de escenas para reasignar referencias
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Importante: desuscribirse del evento para evitar memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Start()
    {
        // Inicializar la vida al máximo
        currentHealth = maxHealth;
        initialPosition = transform.position;

        // Buscar y asignar el panel de Game Over en la escena actual
        FindAndAssignGameOverPanel();

        // Actualizar la UI
        if (ConsumoCafe.Instance != null)
        {
            ConsumoCafe.Instance.UpdateVida(currentHealth);
        }
    }

    // **NUEVO MÉTODO**: Buscar y asignar el panel de Game Over
    private void FindAndAssignGameOverPanel()
    {
        if (gameOverPanel == null)
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas != null)
            {
                Transform panelTransform = canvas.transform.Find("GameOver");
                if (panelTransform != null)
                {
                    gameOverPanel = panelTransform.gameObject;
                    // Asegurarse de que esté desactivado al inicio
                    if (gameOverPanel != null)
                        gameOverPanel.SetActive(false);
                }
            }
        }
    }

    // **NUEVO MÉTODO**: Se ejecuta cada vez que se carga una nueva escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Escena cargada: " + scene.name + ", reasignando referencias...");
        
        // Reasignar el panel de Game Over para la nueva escena
        FindAndAssignGameOverPanel();
        
        // Si el jugador estaba muerto, revivirlo
        if (currentHealth <= 0)
        {
            RespawnPlayer();
        }
    }

    public void ChangeHealth(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;

        // Clamping (asegurar que no pase de 0 ni del máximo)
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // **AQUÍ ESTÁ LA CLAVE: NOTIFICAR AL CANVAS**
        if (ConsumoCafe.Instance != null)
        {
            ConsumoCafe.Instance.UpdateVida(currentHealth);
        }

        if(currentHealth <= 0)
        {
            // Lógica de Game Over
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        Debug.Log("Player has died! Activating Game Over Screen.");

        // 1. **Mostrar el Panel de Game Over**
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("GameOverPanel no asignado en PlayerHealth");
        }

        // 2. **Pausar el juego** (detiene el tiempo y el movimiento)
        Time.timeScale = 0f; 

        // 3. **Opcional:** Desactivar los controles del jugador si es necesario
        // Ejemplo: GetComponent<Jugador1>().enabled = false;
    }

    // **NUEVO MÉTODO**: Para revivir al jugador
    public void RespawnPlayer()
    {
        currentHealth = maxHealth;
        
        // Reubicar en posición inicial
        transform.position = initialPosition;
        
        // Reactivar GameObject si estaba desactivado
        gameObject.SetActive(true);
        
        // Reanudar el tiempo
        Time.timeScale = 1f;
        
        // Ocultar panel de Game Over
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        // Actualizar UI
        if (ConsumoCafe.Instance != null)
        {
            ConsumoCafe.Instance.UpdateVida(currentHealth);
        }
        
        Debug.Log("Player respawned!");
    }
}