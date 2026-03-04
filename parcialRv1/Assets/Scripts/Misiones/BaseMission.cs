using UnityEngine;

/// <summary>
/// Clase base de todas las misiones.
/// Cada tipo de misión hereda de esta clase.
/// </summary>
public abstract class BaseMission : MonoBehaviour
{
    [Header("Misión")]
    public string missionName        = "Misión";
    public string missionDescription = "Descripción de la misión";

    [Header("Visual")]
    [Tooltip("Ícono que aparece en el HUD para esta misión")]
    public Sprite missionIcon;

    public bool IsCompleted { get; protected set; } = false;

    protected virtual void Start()
    {
        // Se registra automáticamente en el MissionManager al iniciar
        if (MissionManager.Instance != null)
            MissionManager.Instance.Register(this);
        else
            Debug.LogWarning($"[{missionName}] No se encontró MissionManager en la escena.");
    }

    // Llamar este método cuando la misión se completa
    protected void Complete()
    {
        if (IsCompleted) return;
        IsCompleted = true;
        OnCompleted();
        MissionManager.Instance?.NotifyCompleted(this);
    }

    // Cada misión puede hacer algo especial al completarse (opcional)
    protected virtual void OnCompleted() { }
}
