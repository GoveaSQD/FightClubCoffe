using UnityEngine;

public class CameraPersister : MonoBehaviour
{
    private static CameraPersister instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);   // 🔥 Si ya existe una cámara persistente, destruye esta
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Solo se ejecuta una vez
    }
}
