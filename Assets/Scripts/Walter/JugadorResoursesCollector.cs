using UnityEngine;
public class JugadorResoursesCollector : MonoBehaviour
{
    // Contadores internos de ítems
    private int cafeBlanco;
    private int capuchino;
    private int cafeNegro;
    private int b52;

    void Start()
    {
        // Inicializa el UI del inventario con contadores a 0 al inicio del juego
        UIManager.Instance.UpdateCafeBlanco(cafeBlanco);
        UIManager.Instance.UpdateCapuchino(capuchino);
        UIManager.Instance.UpdateCafeNegro(cafeNegro);
        UIManager.Instance.UpdateB52(b52);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        bool collected = true; // Bandera para saber si se recogió algo

        if (collision.gameObject.CompareTag("CafeBlanco"))
        {
            cafeBlanco++;
            UIManager.Instance.UpdateCafeBlanco(cafeBlanco);
        }
        else if (collision.gameObject.CompareTag("Capuchino"))
        {
            capuchino++;
            UIManager.Instance.UpdateCapuchino(capuchino);
        }
        else if (collision.gameObject.CompareTag("CafeNegro"))
        {
            cafeNegro++;
            UIManager.Instance.UpdateCafeNegro(cafeNegro);
        }
        else if (collision.gameObject.CompareTag("B52"))
        {
            b52++;
            UIManager.Instance.UpdateB52(b52);
        }
        else
        {
            collected = false; // No es un ítem coleccionable
        }

        if (collected)
        {
            Destroy(collision.gameObject);
        }
    }

    // --- Métodos Públicos para Consumo ---
    
    // Devuelve la cantidad actual de un ítem
    public int GetItemCount(string itemTag)
    {
        switch (itemTag)
        {
            case "CafeBlanco": return cafeBlanco;
            case "Capuchino": return capuchino;
            case "CafeNegro": return cafeNegro;
            case "B52": return b52;
            default: return 0;
        }
    }

    // Reduce la cantidad de un ítem y actualiza la UI del inventario
    public void DecreaseItemCount(string itemTag)
    {
        switch (itemTag)
        {
            case "CafeBlanco":
                cafeBlanco--;
                UIManager.Instance.UpdateCafeBlanco(cafeBlanco);
                break;
            case "Capuchino":
                capuchino--;
                UIManager.Instance.UpdateCapuchino(capuchino);
                break;
            case "CafeNegro":
                cafeNegro--;
                UIManager.Instance.UpdateCafeNegro(cafeNegro);
                break;
            case "B52":
                b52--;
                UIManager.Instance.UpdateB52(b52);
                break;
        }
    }
}