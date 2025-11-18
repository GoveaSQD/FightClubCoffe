using UnityEngine;

[CreateAssetMenu(fileName = "NuevoPersonaje", menuName = "Personaje")]

public class Personajes : ScriptableObject
{
    
    public GameObject personajeJugable;
    public Sprite imagen;
   public RuntimeAnimatorController animacion;
    public Sprite nombre;


   
   
}
