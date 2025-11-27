using UnityEngine;

public class CameraPersister : MonoBehaviour
{
    // Usa Awake para que se ejecute antes que Start, 
    // asegurando que la cámara esté lista para la escena.
    private void Awake()
    {
        // 🔑 Hacemos que el GameObject de la cámara persista
        DontDestroyOnLoad(gameObject);
        
        // OPCIONAL: Si quieres asegurar que solo exista UNA cámara, 
        // puedes añadir una lógica Singleton simple aquí también.
    }
}