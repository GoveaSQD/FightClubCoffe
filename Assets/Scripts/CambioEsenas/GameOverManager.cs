using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public void RestartGame()
    {
        Time.timeScale = 1f;

        // Destruir cualquier ScreenFader persistente antes de recargar
        ScreenFader[] faders = Object.FindObjectsByType<ScreenFader>(FindObjectsSortMode.None);
        foreach (ScreenFader fader in faders)
        {
            // No destruir la instancia actual si existe
            if (fader != ScreenFader.Instance)
            {
                Destroy(fader.gameObject);
            }
        }

        // Recargar la escena actual
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

        StartCoroutine(ReassignReferencesAfterSceneLoad());
    }
    private IEnumerator ReassignReferencesAfterSceneLoad()
    {
        // Espera un frame para que la escena cargue
        yield return null;

        // 1. Obtener al jugador persistente
        PlayerHealth player = Object.FindAnyObjectByType<PlayerHealth>();

        if (player != null)
        {
            player.gameObject.SetActive(true);

            // Restaurar vida
            player.currentHealth = player.maxHealth;
            ConsumoCafe.Instance.UpdateVida(player.maxHealth);

            // Reubicar al jugador
            player.transform.position = new Vector3(-17f, -22f, 0f);
        }

        // 2. Buscar de nuevo el Canvas de esta escena
        GameObject newCanvas = GameObject.Find("Canvas");

        if (newCanvas != null)
        {
            Transform panel = newCanvas.transform.Find("GameOver");

            if (panel != null && player != null)
            {
                player.gameOverPanel = panel.gameObject;
                panel.gameObject.SetActive(false);
            }
        }
    }

public void SalirDelJuego()
{
    Time.timeScale = 1f;

    // 🔥 1. Buscar y destruir Player persistente
    PlayerHealth player = Object.FindAnyObjectByType<PlayerHealth>();
    if (player != null)
    {
        if (player.gameOverPanel != null)
            player.gameOverPanel.SetActive(false);
        Destroy(player.gameObject);
    }

    // 🔥 2. Destruir ScreenFader persistente (CORREGIDO)
    ScreenFader[] faders = Object.FindObjectsByType<ScreenFader>(FindObjectsSortMode.None);
    foreach (ScreenFader fader in faders)
    {
        // Verificar si el objeto existe y no es null
        if (fader != null && fader.gameObject != null)
        {
            Destroy(fader.gameObject);
        }
    }

    // 🔥 3. Destruir objetos persistentes de forma más segura
    DestroyPersistentObjects();

    // 🔥 4. Cargar el menú
    SceneManager.LoadScene("Menu");
}

// Método auxiliar para destruir objetos persistentes
private void DestroyPersistentObjects()
{
    // Encontrar todos los GameObjects en la escena DontDestroyOnLoad
    GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
    
    foreach (GameObject obj in allObjects)
    {
        // Verificar si el objeto es persistente (está en la escena DontDestroyOnLoad)
        if (obj != null && obj.scene.name == "DontDestroyOnLoad")
        {
            // Excluir objetos del sistema de Unity que no debemos destruir
            if (obj.name == "EventSystem" || 
                obj.name.Contains("Unity") ||
                obj.GetComponent<AudioListener>() != null)
            {
                continue; // Saltar estos objetos
            }
            
            // Destruir el objeto
            Destroy(obj);
        }
    }
}
}
