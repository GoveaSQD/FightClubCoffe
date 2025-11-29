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
        DontDestroyOnLoad(gameObject);
    }
    public void Start() // Usamos Start para inicializar después de que ConsumoCafe.Instance exista
    {
        // Inicializar la vida al máximo
        currentHealth = maxHealth;

        initialPosition = transform.position;


        // **Actualizar la UI por primera vez**
        // AVISO: Asegúrate de que ConsumoCafe.Instance esté inicializado antes que esto
        ConsumoCafe.Instance.UpdateVida(currentHealth);
    }

    public void ChamgeHealth(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;

        // Clamping (asegurar que no pase de 0 ni del máximo)
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // **AQUÍ ESTÁ LA CLAVE: NOTIFICAR AL CANVAS**
        ConsumoCafe.Instance.UpdateVida(currentHealth);

        if(currentHealth <= 0 )
        {
            //  Lógica de Game Over
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

        // 2. **Pausar el juego** (detiene el tiempo y el movimiento)
        Time.timeScale = 0f; 

        // 3. **Opcional:** Desactivar los controles del jugador si es necesario
        // Ejemplo: GetComponent<Jugador1>().enabled = false;
    }


}
