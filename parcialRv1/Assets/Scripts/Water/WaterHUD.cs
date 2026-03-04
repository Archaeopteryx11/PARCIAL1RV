using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// HUD que muestra el nivel del agua a los jugadores en tiempo real.
/// Muestra una barra vertical que sube, cambia de color y parpadea en alerta.
///
/// SETUP en Unity:
/// 1. Adjunta a un Canvas GameObject en modo Screen Space - Overlay
/// 2. Crea una Image (type: Filled, Fill Method: Vertical) para la barra
/// 3. Asigna los campos desde el inspector
/// </summary>
public class WaterHUD : MonoBehaviour
{
    [Header("Referencias")]
    public WaterManager waterManager;

    [Header("Barra de Agua")]
    [Tooltip("Image con Fill Method: Vertical (sube de abajo a arriba)")]
    public Image waterBar;
    [Tooltip("Ícono o panel contenedor de la barra")]
    public RectTransform barContainer;

    [Header("Texto de Estado")]
    [Tooltip("Texto que muestra el porcentaje o un mensaje de estado")]
    public TextMeshProUGUI statusText;

    [Header("Colores de la Barra")]
    public Color colorSafe = new Color(0.2f, 0.6f, 1f);
    public Color colorWarning = new Color(1f, 0.75f, 0f);
    public Color colorDanger = new Color(1f, 0.2f, 0.1f);

    [Header("Parpadeo en Alerta")]
    [Tooltip("A qué progreso (0-1) empieza a parpadear")]
    [Range(0f, 1f)] public float blinkThreshold = 0.75f;
    public float blinkSpeed = 3f;

    // ── Internos ─────────────────────────────────────────────────
    private bool isBlinking = false;
    private float blinkTimer = 0f;

    void Start()
    {
        if (waterManager == null)
            waterManager = FindObjectOfType<WaterManager>();
    }

    void Update()
    {
        if (waterManager == null || waterBar == null) return;

        float progress = waterManager.WaterProgress;

        // Actualizar barra
        waterBar.fillAmount = progress;

        // Color gradiente según nivel
        if (progress < 0.5f) waterBar.color = Color.Lerp(colorSafe, colorWarning, progress / 0.5f);
        else if (progress < blinkThreshold) waterBar.color = Color.Lerp(colorWarning, colorDanger, (progress - 0.5f) / 0.25f);
        else waterBar.color = colorDanger;

        // Parpadeo cuando está en zona de peligro
        if (progress >= blinkThreshold)
        {
            blinkTimer += Time.deltaTime * blinkSpeed;
            float alpha = Mathf.Abs(Mathf.Sin(blinkTimer));
            Color c = waterBar.color;
            c.a = Mathf.Lerp(0.4f, 1f, alpha);
            waterBar.color = c;
        }

        // Texto de estado
        if (statusText != null)
        {
            int percent = Mathf.RoundToInt(progress * 100f);
            if (progress < 0.5f) statusText.text = $"Agua: {percent}%";
            else if (progress < blinkThreshold) statusText.text = $"⚠ Agua: {percent}%";
            else statusText.text = $"🚨 ¡PELIGRO! {percent}%";
        }
    }
}
