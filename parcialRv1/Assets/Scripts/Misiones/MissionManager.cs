using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Gestiona las 5 misiones del nivel.
/// Todas activas desde el inicio.
/// Al completarse todas → llama a WaterManager para detener el agua.
///
/// SETUP:
/// 1. Crea un GameObject vacío llamado "MissionManager"
/// 2. Adjunta este script
/// 3. Asigna el WaterManager
/// 4. Las misiones se registran solas con MissionManager.Instance.Register()
/// </summary>
public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [Header("Referencia")]
    public WaterManager waterManager;

    [Header("Eventos")]
    public UnityEvent OnAllMissionsComplete;   // Se dispara al completar todas
    public UnityEvent<BaseMission> OnMissionCompleted; // Se dispara al completar una

    // Lista de todas las misiones registradas en la escena
    private List<BaseMission> missions = new List<BaseMission>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // Las misiones se registran solas al iniciar
    public void Register(BaseMission mission)
    {
        if (!missions.Contains(mission))
            missions.Add(mission);
    }

    // Llamado por cada misión cuando se completa
    public void NotifyCompleted(BaseMission mission)
    {
        Debug.Log($"[MissionManager] Misión completada: {mission.missionName}");

        // Acelerar el agua al completar cada misión
        if (waterManager != null)
            waterManager.OnMissionCompleted();

        OnMissionCompleted?.Invoke(mission);

        // Verificar si todas están completas
        bool allDone = true;
        foreach (var m in missions)
            if (!m.IsCompleted) { allDone = false; break; }

        if (allDone)
        {
            Debug.Log("[MissionManager] ¡Todas las misiones completadas! Victoria.");
            if (waterManager != null) waterManager.PauseWater();
            OnAllMissionsComplete?.Invoke();
        }
    }

    public List<BaseMission> GetAllMissions() => missions;

    public int CompletedCount()
    {
        int count = 0;
        foreach (var m in missions) if (m.IsCompleted) count++;
        return count;
    }
}
