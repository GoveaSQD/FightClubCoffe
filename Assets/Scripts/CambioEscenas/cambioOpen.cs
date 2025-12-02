using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class cambioOpen : MonoBehaviour
{

    public Animator animadorTransicion;
    public float tiempoTransicion = 1.0f;

    public void CargarEscena(int indiceEscena)
    {
        LimpiarObjetosPersistentes();
        SceneManager.LoadScene(indiceEscena);
    }

    // public void CargarEscenaPorNombre(string nombreEscena)
    // {
    //     LimpiarObjetosPersistentes();
    //     SceneManager.LoadScene(nombreEscena);
    // }
    public void CargarEscenaPorNombre(string nombreEscena)
    {
        LimpiarObjetosPersistentes();
        StartCoroutine(TransicionarYcargar(nombreEscena));
    }

    private IEnumerator TransicionarYcargar(object escena)
    {
        // 1. Inicia la animación de Fade Out
        if (animadorTransicion != null)
        {
            // El nombre del Trigger debe coincidir con el que tienes en tu Animator
            animadorTransicion.SetTrigger("StartFadeOut"); 
            
            // 2. Espera a que la animación termine
            yield return new WaitForSeconds(tiempoTransicion);
        }

        // 3. Destruye los objetos persistentes
        LimpiarObjetosPersistentes();

        // 4. Carga la nueva escena
        if (escena is int indice)
        {
            SceneManager.LoadScene(indice);
        }
        else if (escena is string nombre)
        {
            SceneManager.LoadScene(nombre);
        }
    }

    private void LimpiarObjetosPersistentes()
    {
        // Destruir CANVAS persistentes
        Canvas[] allCanvas = FindObjectsOfType<Canvas>(true);
        foreach (var c in allCanvas)
        {
            if (c.gameObject.scene.name == "DontDestroyOnLoad")
                Destroy(c.gameObject);
        }

        // Destruir CAMARAS persistentes
        Camera[] cams = FindObjectsOfType<Camera>(true);
        foreach (var cam in cams)
        {
            if (cam.gameObject.scene.name == "DontDestroyOnLoad")
                Destroy(cam.gameObject);
        }

        // Destruir PLAYER persistente
        var player = GameObject.FindWithTag("Player");
        if (player != null && player.scene.name == "DontDestroyOnLoad")
            Destroy(player);
    }
    
}
