using UnityEngine;

public class JugadorCollector : MonoBehaviour
{
    
    private int cafeBlanco;
    private int capuchino;
    private int cafeNegro;
    private int b52;

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("CafeBlanco"))
        {
            Destroy(collision.gameObject);
            cafeBlanco++;
            Debug.Log("Cafe blanco: " + cafeBlanco);
            UIManager.Instance.UpdateCafeBlanco(cafeBlanco);
        }

        if(collision.gameObject.CompareTag("Capuchino"))
        {
            Destroy(collision.gameObject);
            capuchino++;
            Debug.Log("Capuchino: " + capuchino);
            UIManager.Instance.UpdateCapuchino(capuchino);
        }

        if(collision.gameObject.CompareTag("CafeNegro"))
        {
            Destroy(collision.gameObject);
            cafeNegro++;
            Debug.Log("Cafe Negro: " + cafeNegro);
            UIManager.Instance.UpdateCafeNegro(cafeNegro);
        }

        if(collision.gameObject.CompareTag("B52"))
        {
            Destroy(collision.gameObject);
            b52++;
            Debug.Log("b52: " + b52);
            UIManager.Instance.UpdateB52(b52);
        }
    }
}
