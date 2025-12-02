using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Usamos 'static' para que sea accesible fácilmente
    public static LevelManager Instance { get; private set; }

    public GameObject player;
    // public Vector3 positionLoby;
    // Contador de enemigos
    private int _enemiesInLevel = 0;

    [SerializeField] private GameObject _victoryPanel; // Referencia al panel UI

    private void Awake()
    {
        // Implementación del patrón Singleton básico
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Asegúrate de que el panel de victoria esté inicialmente inactivo
        if (_victoryPanel != null)
        {
            _victoryPanel.SetActive(false);
        }

        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // En Start, encuentra todos los objetos con la etiqueta "Enemigo"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemigo");
        _enemiesInLevel = enemies.Length;

        // Si no hay enemigos al inicio (nivel de tutorial o error), muestra la victoria inmediatamente
        if (_enemiesInLevel == 0)
        {
            CheckForLevelCompletion();
        }

        Debug.Log("Enemigos encontrados al inicio: " + _enemiesInLevel);
    }

    // Método público para llamar cuando un enemigo muere
    public void EnemyDefeated()
    {
        _enemiesInLevel--; // Disminuye el contador
        Debug.Log("Enemigo derrotado. Enemigos restantes: " + _enemiesInLevel);

        // Revisa si es momento de terminar el nivel
        CheckForLevelCompletion();
    }

    private void CheckForLevelCompletion()
    {
        if (_enemiesInLevel <= 0)
        {
            // ¡El jugador ha ganado!
            ShowVictoryScreen();
        }
    }

    private void ShowVictoryScreen()
    {
        Debug.Log("¡Nivel Completado!");
        if (_victoryPanel != null)
        {
            _victoryPanel.SetActive(true); // Activa el panel de victoria
            // También puedes pausar el juego aquí: Time.timeScale = 0f;
        }
    }

    public void NextLevel(string nombreEscena)
    {
        Time.timeScale = 1f;

        _victoryPanel.SetActive(false);

         PlayerHealth player = FindAnyObjectByType<PlayerHealth>();

       if (player != null)
        {
            // Usar el método Respawn del PlayerHealth
            player.CambioALoby();
        }

        SceneManager.LoadScene(nombreEscena);
    }
}