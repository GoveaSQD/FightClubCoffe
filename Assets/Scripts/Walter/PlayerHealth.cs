using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    
    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void Start() // Usamos Start para inicializar después de que ConsumoCafe.Instance exista
    {
        // Inicializar la vida al máximo
        currentHealth = maxHealth;

        // **Actualizar la UI por primera vez**
        // AVISO: Asegúrate de que ConsumoCafe.Instance esté inicializado antes que esto
        ConsumoCafe.Instance.UpdateVida(currentHealth);
    }

    public void ChangeHealth(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;

        // Clamping (asegurar que no pase de 0 ni del máximo)
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // **AQUÍ ESTÁ LA CLAVE: NOTIFICAR AL CANVAS**
        ConsumoCafe.Instance.UpdateVida(currentHealth);

        if(currentHealth <= 0 )
        {
            gameObject.SetActive(false);
        }
    }


}
