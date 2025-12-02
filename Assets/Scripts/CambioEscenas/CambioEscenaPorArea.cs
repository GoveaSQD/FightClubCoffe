using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CambioEscenaPorArea : MonoBehaviour
{
    [Tooltip("El nombre de la escena a cargar (ej: Menu)")]
    public string sceneToLoad = "Menu";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(LoadSceneTransitionAsync());
        }
    }

    private IEnumerator LoadSceneTransitionAsync()
    {
        // 1. Iniciar Fade Out (la pantalla se oscurece)
        yield return ScreenFader.Instance.FadeOut("Comienza la pelea...");

        // 2. Iniciar la carga asíncrona de la nueva escena.
        // allowSceneActivation = false mantiene la escena entrante en pausa al 90%
        // para que no se active hasta que se lo indiquemos.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        // Esperar mientras la carga esté en progreso (y el Fade Out ya terminó)
        while (asyncLoad.progress < 0.9f)
        {
            // Opcional: mostrar progreso de carga aquí
            yield return null;
        }

        // 3. Cuando la carga está lista (90%), activamos la escena.
        asyncLoad.allowSceneActivation = true;
        
        // Esperar un frame para asegurarnos de que la escena cargó completamente
        yield return null; 

        // 4. Realizar Fade In (la pantalla se aclara)
        yield return ScreenFader.Instance.FadeIn();
    }
}