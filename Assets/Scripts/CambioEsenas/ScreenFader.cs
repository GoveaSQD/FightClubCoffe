using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [Header("Referencias UI")]
    public Image fadePanel;              // arrastra aquí la Image negra (Fill: Stretch)
    public TextMeshProUGUI textoPelea;   // arrastra aquí tu TMP Text (opcional)
    
    // **NUEVO**: Referencia al componente Canvas principal
    private Canvas faderCanvas;

    [Header("Duraciones")]
    public float fadeOutDuration = 2f;
    public float fadeInDuration = 1.5f;

    private void Awake()
    {
        // Implementación Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Obtener la referencia al componente Canvas
        faderCanvas = GetComponent<Canvas>();
        if (faderCanvas == null)
        {
            Debug.LogError("ScreenFader necesita un componente Canvas adjunto.");
            return;
        }

        // Asegurar estado inicial transparente
        if (fadePanel != null)
            fadePanel.color = new Color(0, 0, 0, 0);

        if (textoPelea != null)
            textoPelea.color = new Color(textoPelea.color.r, textoPelea.color.g, textoPelea.color.b, 0);

        // **IMPORTANTE**: Desactivamos el Canvas al inicio para que no interfiera.
        faderCanvas.enabled = false;
    }

    // Ahora acepta un mensaje opcional. Si no quieres texto, no pases nada.
    public IEnumerator FadeOut(string message = "")
    {
        // **IMPORTANTE**: Activamos el Canvas antes de empezar a mostrar la transición.
        if (faderCanvas != null)
            faderCanvas.enabled = true;
        
        if (textoPelea != null)
        {
            textoPelea.text = message;
            textoPelea.gameObject.SetActive(!string.IsNullOrEmpty(message));
            // También aseguramos que el texto empiece transparente si hay mensaje
            if (textoPelea.gameObject.activeSelf)
                textoPelea.color = new Color(textoPelea.color.r, textoPelea.color.g, textoPelea.color.b, 0); 
        }

        float t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / fadeOutDuration);

            // Transición a opaco (alpha: 0 -> 1)
            if (fadePanel != null)
                fadePanel.color = new Color(0, 0, 0, a);

            if (textoPelea != null && textoPelea.gameObject.activeSelf)
                textoPelea.color = new Color(textoPelea.color.r, textoPelea.color.g, textoPelea.color.b, a);

            yield return null;
        }

        // Asegurar opacidad final
        if (fadePanel != null)
            fadePanel.color = new Color(0, 0, 0, 1);

        if (textoPelea != null && textoPelea.gameObject.activeSelf)
            textoPelea.color = new Color(textoPelea.color.r, textoPelea.color.g, textoPelea.color.b, 1);
    }

    public IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            float a = 1f - Mathf.Clamp01(t / fadeInDuration);

            // Transición a transparente (alpha: 1 -> 0)
            if (fadePanel != null)
                fadePanel.color = new Color(0, 0, 0, a);

            if (textoPelea != null && textoPelea.gameObject.activeSelf)
                textoPelea.color = new Color(textoPelea.color.r, textoPelea.color.g, textoPelea.color.b, a);

            yield return null;
        }

        // Asegurar transparente al terminar
        if (fadePanel != null)
            fadePanel.color = new Color(0, 0, 0, 0);

        if (textoPelea != null)
        {
            textoPelea.color = new Color(textoPelea.color.r, textoPelea.color.g, textoPelea.color.b, 0);
            textoPelea.gameObject.SetActive(false);
        }

        // **IMPORTANTE**: Desactivamos el Canvas por completo. Esto evita que se renderice
        // y bloquee la nueva escena.
        if (faderCanvas != null)
            faderCanvas.enabled = false;
    }
}