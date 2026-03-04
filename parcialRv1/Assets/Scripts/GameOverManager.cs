using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Gestiona toda la secuencia de Game Over:
///   1. Sonido de alerta cuando el agua está al 75%
///   2. Efecto visual: la pantalla se "llena de agua" (overlay azul)
///   3. Pantalla de Game Over con UI
///   4. Reinicio automático de la escena
///
/// SETUP en Unity:
/// 1. Adjunta este script a un Canvas GameObject
/// 2. Crea los elementos UI indicados abajo y asígnalos
/// 3. Suscribe OnAlertLevel de WaterManager -> TriggerAlert()
/// 4. Suscribe OnGameOver  de WaterManager -> TriggerGameOver()
/// </summary>
public class GameOverManager : MonoBehaviour
{
    [Header("Panel Game Over")]
    [Tooltip("Panel completo de Game Over (inicialmente desactivado)")]
    public GameObject gameOverPanel;
    [Tooltip("Texto del título: 'GAME OVER' o '¡El agua subió demasiado!'")]
    public TextMeshProUGUI titleText;
    [Tooltip("Texto secundario con instrucción (ej: 'Reiniciando...')")]
    public TextMeshProUGUI subtitleText;
    [Tooltip("Barra de progreso del conteo regresivo antes del reinicio")]
    public Slider countdownSlider;

    [Header("Efecto de Agua en Pantalla")]
    [Tooltip("Imagen fullscreen semitransparente azul (el 'agua que sube en cámara')")]
    public Image waterOverlay;
    [Tooltip("Color del overlay de agua (alpha 0 = invisible, 1 = opaco)")]
    public Color waterOverlayColor = new Color(0.1f, 0.4f, 0.9f, 0f);

    [Header("Audio")]
    [Tooltip("Clip de alarma que suena al llegar al 75% del agua")]
    public AudioClip alertClip;
    [Tooltip("Clip que suena al Game Over (agua inundando)")]
    public AudioClip gameOverClip;
    [Tooltip("AudioSource para los clips de alerta")]
    public AudioSource audioSource;

    [Header("Configuración")]
    [Tooltip("Segundos de espera antes de reiniciar automáticamente")]
    public float restartDelay = 4f;
    [Tooltip("Velocidad a la que el overlay de agua sube en pantalla")]
    public float overlayFillSpeed = 0.8f;

    // ── Estado interno ───────────────────────────────────────────
    private bool gameOverTriggered = false;

    void Awake()
    {
        // Asegurar estado inicial limpio
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (waterOverlay  != null)
        {
            waterOverlayColor.a = 0f;
            waterOverlay.color  = waterOverlayColor;
            waterOverlay.gameObject.SetActive(true);
        }
    }

    // ── API Pública (conectar a eventos de WaterManager) ─────────

    /// <summary>
    /// Conectar a WaterManager.OnAlertLevel
    /// Suena la alarma cuando el agua llega al umbral de peligro.
    /// </summary>
    public void TriggerAlert()
    {
        if (audioSource != null && alertClip != null)
            audioSource.PlayOneShot(alertClip);

        Debug.Log("[GameOverManager] Alerta de nivel de agua activada.");
    }

    /// <summary>
    /// Conectar a WaterManager.OnGameOver
    /// Inicia toda la secuencia de Game Over.
    /// </summary>
    public void TriggerGameOver()
    {
        if (gameOverTriggered) return;
        gameOverTriggered = true;

        Debug.Log("[GameOverManager] Iniciando secuencia Game Over.");
        StartCoroutine(GameOverSequence());
    }

    // ── Coroutines ───────────────────────────────────────────────

    private IEnumerator GameOverSequence()
    {
        // 1. Sonido de Game Over
        if (audioSource != null && gameOverClip != null)
            audioSource.PlayOneShot(gameOverClip);

        // 2. Efecto visual: el overlay de agua llena la pantalla
        yield return StartCoroutine(FillWaterOverlay());

        // 3. Mostrar panel de Game Over
        ShowGameOverUI();

        // 4. Cuenta regresiva y reinicio automático
        yield return StartCoroutine(CountdownAndRestart());
    }

    private IEnumerator FillWaterOverlay()
    {
        if (waterOverlay == null) yield break;

        float elapsed = 0f;
        float duration = 1f / overlayFillSpeed;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);

            // El overlay simula el agua llenando la cámara (sube desde abajo)
            waterOverlayColor.a = alpha * 0.75f; // máximo 75% de opacidad
            waterOverlay.color  = waterOverlayColor;

            // Efecto: el fillAmount del Image sube desde 0 a 1 (requiere Image type: Filled)
            if (waterOverlay.type == Image.Type.Filled)
                waterOverlay.fillAmount = alpha;

            yield return null;
        }
    }

    private void ShowGameOverUI()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (titleText    != null) titleText.text    = "¡EL AGUA SUBIÓ DEMASIADO!";
        if (subtitleText != null) subtitleText.text = "Reiniciando nivel...";
        if (countdownSlider != null)
        {
            countdownSlider.minValue = 0f;
            countdownSlider.maxValue = restartDelay;
            countdownSlider.value    = restartDelay;
        }
    }

    private IEnumerator CountdownAndRestart()
    {
        float remaining = restartDelay;

        while (remaining > 0f)
        {
            remaining -= Time.deltaTime;

            if (countdownSlider != null)
                countdownSlider.value = remaining;

            if (subtitleText != null)
                subtitleText.text = $"Reiniciando en {Mathf.CeilToInt(remaining)}...";

            yield return null;
        }

        RestartLevel();
    }

    private void RestartLevel()
    {
        Debug.Log("[GameOverManager] Reiniciando escena...");
        // Time.timeScale por si se pausó durante el Game Over
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ── Método opcional para reinicio manual (botón en UI) ───────
    public void OnRestartButtonPressed()
    {
        StopAllCoroutines();
        RestartLevel();
    }
}
