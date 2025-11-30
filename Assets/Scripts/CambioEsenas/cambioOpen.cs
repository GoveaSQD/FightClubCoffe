using UnityEngine;
using UnityEngine.SceneManagement;

public class cambioOpen : MonoBehaviour
{
    public void CargarEscena(int indiceEscena)
    {
        LimpiarObjetosPersistentes();
        SceneManager.LoadScene(indiceEscena);
    }

    public void CargarEscenaPorNombre(string nombreEscena)
    {
        LimpiarObjetosPersistentes();
        SceneManager.LoadScene(nombreEscena);
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
