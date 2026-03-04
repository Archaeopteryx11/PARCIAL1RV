using UnityEngine;

/// <summary>
/// Anima las propiedades del shader de agua en tiempo real:
/// ondas, espuma, color y transparencia.
///
/// SETUP en Unity:
/// 1. Adjunta al mismo GameObject que tiene el Renderer del agua
/// 2. El Material del agua debe usar el shader con las properties indicadas
///    (puedes usar el Shader Graph de Unity o el shader HLSL incluido abajo)
/// 3. WaterManager.UpdateShaderLevel() ya llama _WaterLevel y _Turbidity,
///    este script maneja el resto de la animación continua
///
/// PROPERTIES que debe tener el shader del material:
///   _WaterLevel   (Float 0-1) : nivel relativo del agua
///   _Turbidity    (Float 0-1) : qué tan turbia/oscura está el agua
///   _WaveSpeed    (Float)     : velocidad de las olas
///   _WaveHeight   (Float)     : altura de las olas
///   _FoamAmount   (Float 0-1) : espuma en los bordes
///   _BaseColor    (Color)     : color base del agua
///   _DeepColor    (Color)     : color cuando está turbia/profunda
/// </summary>
[RequireComponent(typeof(Renderer))]
public class WaterShaderController : MonoBehaviour
{
    [Header("Referencia")]
    [Tooltip("Asigna el WaterManager de la escena")]
    public WaterManager waterManager;

    [Header("Animación de Olas")]
    [Tooltip("Velocidad base de animación de las olas")]
    public float waveSpeed = 0.5f;
    [Tooltip("Altura de las olas en estado normal")]
    public float waveHeightNormal = 0.05f;
    [Tooltip("Altura de las olas cuando el agua está en alerta (más agitada)")]
    public float waveHeightAlert = 0.18f;

    [Header("Espuma")]
    [Tooltip("Cantidad de espuma en bordes (estado normal)")]
    [Range(0f, 1f)] public float foamNormal = 0.1f;
    [Tooltip("Cantidad de espuma en estado de alerta")]
    [Range(0f, 1f)] public float foamAlert = 0.55f;

    [Header("Color del Agua")]
    [Tooltip("Color del agua en estado inicial (seguro)")]
    public Color colorSafe = new Color(0.2f, 0.6f, 0.9f, 0.6f);
    [Tooltip("Color del agua en alerta (más oscura, turbia)")]
    public Color colorDanger = new Color(0.05f, 0.15f, 0.4f, 0.85f);

    // ── Internos ─────────────────────────────────────────────────
    private Renderer rend;
    private Material mat;

    // IDs de shader cacheados (más eficiente que strings en Update)
    private int idWaveSpeed = Shader.PropertyToID("_WaveSpeed");
    private int idWaveHeight = Shader.PropertyToID("_WaveHeight");
    private int idFoamAmount = Shader.PropertyToID("_FoamAmount");
    private int idBaseColor = Shader.PropertyToID("_BaseColor");
    private int idDeepColor = Shader.PropertyToID("_DeepColor");
    private int idTimeOffset = Shader.PropertyToID("_TimeOffset");

    // ── Ciclo de vida ─────────────────────────────────────────────
    void Awake()
    {
        rend = GetComponent<Renderer>();
        // Instanciar el material para no modificar el asset original
        mat = rend.material;
    }

    void Start()
    {
        if (waterManager == null)
            waterManager = FindObjectOfType<WaterManager>();

        ApplyShaderValues(0f);
    }

    void Update()
    {
        if (waterManager == null) return;

        float progress = waterManager.WaterProgress;

        // Animar el tiempo del shader (para que las olas se muevan)
        mat.SetFloat(idTimeOffset, Time.time * waveSpeed);

        // Interpolar todos los valores según el progreso del agua
        ApplyShaderValues(progress);
    }

    // ── Métodos privados ─────────────────────────────────────────

    private void ApplyShaderValues(float t)
    {
        // Olas: se agitan más conforme sube el agua
        float currentWaveHeight = Mathf.Lerp(waveHeightNormal, waveHeightAlert, t);
        mat.SetFloat(idWaveHeight, currentWaveHeight);
        mat.SetFloat(idWaveSpeed, waveSpeed + t * 0.8f);

        // Espuma: más espuma = más agitación visual
        float currentFoam = Mathf.Lerp(foamNormal, foamAlert, t);
        mat.SetFloat(idFoamAmount, currentFoam);

        // Color: transición de azul claro a azul oscuro/turbio
        Color currentColor = Color.Lerp(colorSafe, colorDanger, t);
        mat.SetColor(idBaseColor, currentColor);
        mat.SetColor(idDeepColor, Color.Lerp(colorSafe * 0.5f, colorDanger * 0.3f, t));
    }
}
