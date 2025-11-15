using UnityEngine;
using UnityEngine.SceneManagement;
public class cambioOpen : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CargarEscena(int indiceEsena){
        SceneManager.LoadScene(indiceEsena);
    }

    public void CargarEscenaPorNombre(string nombreEscena){
        SceneManager.LoadScene(nombreEscena);
    }
}
