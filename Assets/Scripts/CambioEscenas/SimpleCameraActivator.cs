// Script de emergencia - SimpleCameraActivator.cs
using UnityEngine;

public class SimpleCameraActivator : MonoBehaviour
{
    void Start()
    {
        // Encontrar todas las cámaras
        Camera[] cameras = FindObjectsOfType<Camera>();
        
        if (cameras.Length == 0)
        {
            Debug.LogError("¡NO HAY CÁMARAS EN LA ESCENA!");
            return;
        }
        
        // Activar la primera cámara encontrada
        cameras[0].gameObject.SetActive(true);
        cameras[0].tag = "MainCamera";
        
        Debug.Log("Cámara activada: " + cameras[0].gameObject.name);
        
        // Desactivar las demás
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
            Debug.Log("Cámara desactivada: " + cameras[i].gameObject.name);
        }
    }
}