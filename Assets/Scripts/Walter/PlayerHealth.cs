using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    
    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChamgeHealth(int amount)
    {
        currentHealth += amount;
        if(currentHealth <= 0 )
        {
            gameObject.SetActive(false);
        }
    }

}
