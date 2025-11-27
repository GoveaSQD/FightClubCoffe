using UnityEngine;
using TMPro;

public class ConsumoCafe : MonoBehaviour
{
    public TMP_Text vidaJugador;
    public TMP_Text dano;
    public TMP_Text velocidad;

    // Única declaración de la instancia Singleton
    public static ConsumoCafe Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // --- MÉTODOS DE ACTUALIZACIÓN DE UI ---
    
    public void UpdateVida(int value)
    {
        // vidaJugador.text = value.ToString();
        vidaJugador.text = value.ToString()+"/5 lp";
        
    }

    public void UpdateDano(int value)
    {
        dano.text = value.ToString(); 
    }

    public void UpdateVelocidad(int value)
    {
        velocidad.text = value.ToString();
    }
}