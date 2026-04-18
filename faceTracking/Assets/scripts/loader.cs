using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using DG.Tweening;

public class SplashLoader : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RectTransform barraProgreso;
    [SerializeField] private TextMeshProUGUI textoCarga;
    [SerializeField] private RectTransform logo;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Configuracion")]
    [SerializeField] private float duracionCarga = 3f;
    [SerializeField] private string escenaDestino = "MainScene";

    void Start()
    {
        DataManager.Instance?.RegistrarUsoApp(); //llevar el conteo de inicio desesion 
        // Inicia la carga
        StartCoroutine(CargarEscena());
    }

    IEnumerator CargarEscena()
    {
        // Espera que aparezca el logo
        yield return new WaitForSeconds(1f);

        // Textos de carga elegantes
        string[] mensajes = {
            "Iniciando experiencia...",
            "Preparando filtros...",
            "Calibrando AR...",
            "Bienvenida a LUMIÈRE"
        };

        float tiempoPorMensaje = duracionCarga / mensajes.Length;

        for (int i = 0; i < mensajes.Length; i++)
        {
            // Actualiza texto
            if (textoCarga != null)
                textoCarga.text = mensajes[i];

            // Anima la barra de progreso
            float progreso = (float)(i + 1) / mensajes.Length;
            barraProgreso.DOScaleX(progreso, tiempoPorMensaje)
                         .SetEase(Ease.InOutSine);

            yield return new WaitForSeconds(tiempoPorMensaje);
        }

        // Fade out antes de cambiar escena
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0f, 0.5f);
            yield return new WaitForSeconds(0.5f);
        }

        // Carga la escena principal
        SceneManager.LoadScene(escenaDestino);
    }
}