using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public void RestartGame()
    {
        Time.timeScale = 1f;

        // Buscar al jugador persistente
        PlayerHealth player = FindAnyObjectByType<PlayerHealth>();
        
        if (player != null)
        {
            // Usar el método Respawn del PlayerHealth
            player.RespawnPlayer();
        }
        else
        {
            // Fallback: recargar la escena si no se encuentra el jugador
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }

    public void SalirDelJuego()
    {
        Time.timeScale = 1f;

        // Destruir objetos persistentes de forma segura
        DestroyPersistentObjects();

        // Cargar el menú
        SceneManager.LoadScene("Menu");
    }

    private void DestroyPersistentObjects()
    {
        // Buscar todos los objetos en DontDestroyOnLoad
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            if (obj != null && obj.scene.name == "DontDestroyOnLoad")
            {
                // Excluir objetos del sistema de Unity
                if (obj.name == "EventSystem" || 
                    obj.name.Contains("Unity") ||
                    obj.GetComponent<AudioListener>() != null)
                {
                    continue;
                }
                
                Destroy(obj);
            }
        }
    }
}