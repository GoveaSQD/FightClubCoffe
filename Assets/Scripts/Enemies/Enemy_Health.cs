using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    // public void Awake()
    // {
    //     DontDestroyOnLoad(gameObject);
    // }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        else if(currentHealth <= 0)
        {
            Die();
        }
    }


    private void Die()
    {
        // 1. Notificar al LevelManager
        // Usamos el patrón Singleton (LevelManager.Instance) para acceder al método.
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.EnemyDefeated(); 
        }
        else
        {
            Debug.LogError("El LevelManager no se encontró en la escena. Asegúrate de que existe.");
        }
        
        // 2. Destruir el objeto del enemigo
        Destroy(gameObject);
    }
}
