using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject inventory;
    // public GameObject pauseMenu;//para despausar y pausar el juego
    public TMP_Text cafeBlancoCountText;
    public TMP_Text capuchinoCountText;
    public TMP_Text cafeNegroCountText;
    public TMP_Text b52;

    // public TMP_Text healthText;

    public static UIManager Instance { get; private set; }

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

    public void OpenCloseInventory()
    {
        inventory.SetActive(!inventory.activeSelf);

    }

    public void UpdateCafeBlanco(int value)
    {
        cafeBlancoCountText.text = value.ToString();
    }

    public void UpdateCapuchino(int value)
    {
        capuchinoCountText.text = value.ToString();
    }

    public void UpdateCafeNegro(int value)
    {
        cafeNegroCountText.text = value.ToString();
    }

     public void UpdateB52(int value)
    {
        b52.text = value.ToString();
    }


    // public void UpdateHealth(int hpValue)
    // {
    //     healthText.text = hpValue.ToString();
    // }


    // public void PauseGame()
    // {
    //     pauseMenu.SetActive(true);
    //     //PARA DETENER EL UEGO
    //     Time.timeScale = 0;
    // }
//volver a jugar
    // public void ResumeGame()
    // {
    //     pauseMenu.SetActive(false);
    //     Time.timeScale = 1;
    // }

}
