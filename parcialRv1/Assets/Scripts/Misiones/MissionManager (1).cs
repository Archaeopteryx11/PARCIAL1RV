using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Sistema central de misiones.
/// Gestiona el flujo de misiones activas, registra progreso y notifica al WaterManager.
///
/// SETUP en Unity:
/// 1. Crea un GameObject vacío llamado "MissionManager" en tu escena.
/// 2. Adjunta este script.
/// 3. Arrastra tu WaterManager en el Inspector.
/// 4. Crea ScriptableObjects MissionData (Assets > Create > Missions > MissionData).
/// 5. Agrega las misiones a la lista "missions".
/// </summary>
public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [Header("Referencias")]
    public WaterManager waterManager;

    [Header("Misiones")]
    [Tooltip("Lista de misiones en el orden que se presentarán")]
    public List<MissionData> missions = new List<MissionData>();

    [Tooltip("¿Las misiones se activan todas a la vez o una por una?")]
    public bool sequentialMissions = false;

    [Header("Eventos")]
    public UnityEvent<MissionData> OnMissionStarted;
    public UnityEvent<MissionData> OnMissionCompleted;
    public UnityEvent OnAllMissionsCompleted;

    // ── Estado interno ────────────────────────────────────────────
    private Dictionary<string, MissionInstance> missionInstances = new Dictionary<string, MissionInstance>();
    private int currentSequentialIndex = 0;
    private int completedCount = 0;

    // Propiedad pública para la UI
    public MissionInstance CurrentMission =>
        sequentialMissions && currentSequentialIndex < missions.Count
            ? GetMission(missions[currentSequentialIndex].missionID)
            : null;

    public int TotalMissions => missions.Count;
    public int CompletedMissions => completedCount;

    // ── Ciclo de vida ─────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (waterManager == null)
            waterManager = FindObjectOfType<WaterManager>();

        // Registrar todas las misiones
        foreach (var data in missions)
        {
            if (data == null) continue;
            var instance = new MissionInstance(data);
            missionInstances[data.missionID] = instance;
        }

        // Activar misiones según el modo
        if (sequentialMissions)
            ActivateMission(missions[0].missionID);
        else
            foreach (var data in missions)
                ActivateMission(data.missionID);
    }

    // ── API Pública ───────────────────────────────────────────────

    /// <summary>
    /// Registra progreso en una misión. Llama esto desde tus objetos interactuables.
    /// Ejemplo: MissionManager.Instance.RegisterProgress("mission_cables", 1);
    /// </summary>
    public void RegisterProgress(string missionID, int amount = 1)
    {
        if (!missionInstances.TryGetValue(missionID, out var inst))
        {
            Debug.LogWarning($"[MissionManager] Misión '{missionID}' no encontrada.");
            return;
        }

        if (inst.status != MissionStatus.Active)
        {
            Debug.Log($"[MissionManager] Misión '{missionID}' no está activa (estado: {inst.status}).");
            return;
        }

        inst.currentCount += amount;
        inst.currentCount = Mathf.Clamp(inst.currentCount, 0, inst.data.requiredCount);

        Debug.Log($"[MissionManager] Progreso '{missionID}': {inst.currentCount}/{inst.data.requiredCount}");

        if (inst.currentCount >= inst.data.requiredCount)
            CompleteMission(missionID);
    }

    /// <summary>
    /// Completa una misión directamente (útil para MissionType.Custom).
    /// </summary>
    public void CompleteMission(string missionID)
    {
        if (!missionInstances.TryGetValue(missionID, out var inst)) return;
        if (inst.status == MissionStatus.Completed) return;

        inst.status = MissionStatus.Completed;
        inst.currentCount = inst.data.requiredCount;
        completedCount++;

        Debug.Log($"[MissionManager] ✅ Misión completada: {inst.data.missionTitle}");

        // Notificar al WaterManager → sube velocidad del agua
        waterManager?.OnMissionCompleted();

        // Si la misión reduce el agua como recompensa
        if (inst.data.reducesWater)
            waterManager?.ReduceWaterLevel(inst.data.waterReductionAmount);

        OnMissionCompleted?.Invoke(inst.data);

        // Verificar si todas las misiones están completas
        if (completedCount >= missions.Count)
        {
            OnAllMissionsCompleted?.Invoke();
            Debug.Log("[MissionManager] 🏆 ¡Todas las misiones completadas!");
            return;
        }

        // Modo secuencial: activar la siguiente misión
        if (sequentialMissions)
        {
            currentSequentialIndex++;
            if (currentSequentialIndex < missions.Count)
                ActivateMission(missions[currentSequentialIndex].missionID);
        }
    }

    /// <summary>
    /// Obtiene el estado de una misión por ID.
    /// </summary>
    public MissionInstance GetMission(string missionID)
    {
        missionInstances.TryGetValue(missionID, out var inst);
        return inst;
    }

    /// <summary>
    /// Devuelve todas las misiones activas actualmente.
    /// </summary>
    public List<MissionInstance> GetActiveMissions()
    {
        var active = new List<MissionInstance>();
        foreach (var inst in missionInstances.Values)
            if (inst.status == MissionStatus.Active)
                active.Add(inst);
        return active;
    }

    // ── Privados ──────────────────────────────────────────────────

    private void ActivateMission(string missionID)
    {
        if (!missionInstances.TryGetValue(missionID, out var inst)) return;
        inst.status = MissionStatus.Active;
        OnMissionStarted?.Invoke(inst.data);
        Debug.Log($"[MissionManager] 🎯 Misión activada: {inst.data.missionTitle}");
    }
}

// ── Clases de soporte ─────────────────────────────────────────────

public enum MissionStatus { Inactive, Active, Completed, Failed }

/// <summary>
/// Estado en tiempo real de una misión durante la partida.
/// </summary>
public class MissionInstance
{
    public MissionData data;
    public MissionStatus status = MissionStatus.Inactive;
    public int currentCount = 0;

    public float Progress => data.requiredCount > 0
        ? (float)currentCount / data.requiredCount
        : 0f;

    public MissionInstance(MissionData data)
    {
        this.data = data;
    }
}
