using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuSeleccionPersonaje : MonoBehaviour
{
   private int index;

   [SerializeField] private Image imagen;
    [SerializeField] private Animator animacionPersonaje;
    [SerializeField] private Image nombre;
   private GameManager gameManager;

private void Awake()
    {
        // 🔹 SI NO LO ASIGNASTE EN EL INSPECTOR, LO BUSCA AUTOMÁTICAMENTE
        if (animacionPersonaje == null)
        {
            animacionPersonaje = GetComponentInChildren<Animator>();
        }
    }
   private IEnumerator Start(){
    gameManager = GameManager.Instance;

    index = PlayerPrefs.GetInt("JugadorIndex");

    if(index > gameManager.personajes.Count - 1){
        index = 0;
    }

    yield return null;

    CambiarPantalla();
}


  private void CambiarPantalla()
{
    PlayerPrefs.SetInt("JugadorIndex", index);

    animacionPersonaje.runtimeAnimatorController =gameManager.personajes[index].animacion;
    imagen.sprite = gameManager.personajes[index].imagen;
    nombre.sprite = gameManager.personajes[index].nombre;
}

   public void SiguientePersonaje(){
    if(index == gameManager.personajes.Count -1){
        index = 0;
    }else{
        index +=1;
    }
    CambiarPantalla();
   }

   public void AnteriorPersonaje(){
    if(index == 0){
        index = gameManager.personajes.Count - 1;
    }else{
        index -=1;
    }
    CambiarPantalla();
   }

   public void IniciarJuego(){
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
   }
}
