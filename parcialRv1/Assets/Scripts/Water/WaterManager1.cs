using UnityEngine;
using UnityEngine.Events;


public class WaterManager : MonoBehaviour
{

    public Transform waterMesh;

    [Header("Niveles del Agua")]
    [Tooltip("Posición Y inicial del agua (suelo del subterráneo)")]
    public float waterMinY = -5f;
    [Tooltip("Posición Y donde el nivel mata a los jugadores (techo del subterráneo)")]
    public float waterMaxY = 3f;

    [Header("Velocidad")]
    [Tooltip("Velocidad de subida inicial (unidades Unity por segundo)")]
    public float baseRiseSpeed = 0.05f;
    [Tooltip("Cuánto acelera por cada misión completada")]
    public float speedIncreasePerMission = 0.02f;
    [Tooltip("Velocidad máxima posible (evita que sea injugable)")]
    public float maxRiseSpeed = 0.25f;

    [Header("Alertas")]
    [Tooltip("Porcentaje del nivel (0-1) en que suena la alarma de peligro")]
    [Range(0f, 1f)] public float alertThreshold = 0.75f;

    [Header("Eventos")]
    public UnityEvent OnAlertLevel;   // Se dispara una vez al llegar al umbral
    public UnityEvent OnGameOver;     // Se dispara cuando el agua llega al máximo

    // ── Estado interno ──────────────────────────────────────────
    private float currentSpeed;
    private bool isRunning = false;
    private bool alertFired = false;
    private bool gameOverFired = false;

    // Propiedad pública para que la UI pueda leer el progreso (0-1)
    public float WaterProgress =>
        Mathf.InverseLerp(waterMinY, waterMaxY, waterMesh.position.y);

    // ── Ciclo de vida ────────────────────────────────────────────
    void Start()
    {
        currentSpeed = baseRiseSpeed;

        // Posicionar el agua en su nivel mínimo al inicio
        if (waterMesh != null)
        {
            Vector3 startPos = waterMesh.position;
            startPos.y = waterMinY;
            waterMesh.position = startPos;
        }

        StartWater();
    }

    void Update()
    {
        if (!isRunning || gameOverFired || waterMesh == null) return;

        // Subir el agua
        Vector3 pos = waterMesh.position;
        pos.y += currentSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, waterMinY, waterMaxY);
        waterMesh.position = pos;

        // Actualizar el shader con el nivel actual (para efectos visuales)
        UpdateShaderLevel(WaterProgress);

        // Verificar alerta
        if (!alertFired && WaterProgress >= alertThreshold)
        {
            alertFired = true;
            OnAlertLevel?.Invoke();
            Debug.Log("[WaterManager] ¡Nivel de alerta alcanzado!");
        }

        // Verificar Game Over
        if (!gameOverFired && waterMesh.position.y >= waterMaxY)
        {
            gameOverFired = true;
            isRunning = false;
            OnGameOver?.Invoke();
            Debug.Log("[WaterManager] ¡Game Over! El agua llegó al límite.");
        }
    }

    // ── API Pública ──────────────────────────────────────────────

    /// <summary>
    /// Llamar desde MissionManager cada vez que se completa una misión.
    /// Acelera la velocidad del agua.
    /// </summary>
    public void OnMissionCompleted()
    {
        currentSpeed = Mathf.Min(currentSpeed + speedIncreasePerMission, maxRiseSpeed);
        Debug.Log($"[WaterManager] Misión completada. Nueva velocidad: {currentSpeed:F3}");
    }

    /// <summary>
    /// Reduce el nivel del agua como recompensa (si se usa esa mecánica).
    /// </summary>
    public void ReduceWaterLevel(float amount)
    {
        if (waterMesh == null) return;
        Vector3 pos = waterMesh.position;
        pos.y = Mathf.Max(pos.y - amount, waterMinY);
        waterMesh.position = pos;
    }

    public void StartWater() => isRunning = true;
    public void PauseWater() => isRunning = false;

    /// <summary>
    /// Reinicia el agua a su posición y velocidad iniciales.
    /// Llamar desde GameOverManager antes de recargar la escena.
    /// </summary>
    public void ResetWater()
    {
        currentSpeed = baseRiseSpeed;
        alertFired = false;
        gameOverFired = false;
        isRunning = false;

        if (waterMesh != null)
        {
            Vector3 pos = waterMesh.position;
            pos.y = waterMinY;
            waterMesh.position = pos;
        }
    }

    // ── Shader ───────────────────────────────────────────────────

    private void UpdateShaderLevel(float progress)
    {
        // Envía el progreso al shader para que ajuste ondas, color, transparencia
        // El shader debe tener un Float property llamado "_WaterLevel"
        if (waterMesh.TryGetComponent<Renderer>(out var rend))
        {
            rend.material.SetFloat("_WaterLevel", progress);
            // Oscurecer el agua conforme sube (más turbia = más peligrosa)
            rend.material.SetFloat("_Turbidity", Mathf.Lerp(0f, 1f, progress));
        }
    }

    // ── Gizmos (visualización en editor) ─────────────────────────
    void OnDrawGizmosSelected()
    {
        // Nivel mínimo (azul claro)
        Gizmos.color = new Color(0.3f, 0.6f, 1f, 0.5f);
        Gizmos.DrawWireCube(new Vector3(0, waterMinY, 0), new Vector3(10, 0.05f, 10));

        // Nivel de alerta (amarillo)
        float alertY = Mathf.Lerp(waterMinY, waterMaxY, alertThreshold);
        Gizmos.color = new Color(1f, 0.85f, 0f, 0.5f);
        Gizmos.DrawWireCube(new Vector3(0, alertY, 0), new Vector3(10, 0.05f, 10));

        // Nivel máximo / Game Over (rojo)
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.5f);
        Gizmos.DrawWireCube(new Vector3(0, waterMaxY, 0), new Vector3(10, 0.05f, 10));
    }
}
