using System.Collections;
using UnityEngine;

/// <summary>
/// Zona que completa una misión cuando el jugador entra (o sobrevive X segundos dentro).
/// Usa para misiones tipo: ReachZone o SurviveTime.
///
/// SETUP en Unity:
/// 1. Adjunta a un GameObject con Collider (Is Trigger = true).
/// 2. Asigna el missionID.
/// 3. Para SurviveTime: activa "requireSurvivalTime" y ajusta "survivalSeconds".
/// </summary>
public class MissionZone : MonoBehaviour
{
    [Header("Misión")]
    public string missionID;

    [Header("Tipo de Zona")]
    [Tooltip("Si es false, completa la misión al entrar. Si es true, el jugador debe quedarse X segundos.")]
    public bool requireSurvivalTime = false;
    [Tooltip("Segundos que el jugador debe permanecer en la zona")]
    public float survivalSeconds = 10f;

    [Header("Visual")]
    [Tooltip("Renderer del área (para cambiar color al activarse)")]
    public Renderer zoneRenderer;
    public Color colorIdle = new Color(0.2f, 0.8f, 1f, 0.3f);
    public Color colorActive = new Color(0.1f, 1f, 0.3f, 0.5f);
    public Color colorCompleted = new Color(0f, 1f, 0.2f, 0.7f);

    private bool completed = false;
    private Coroutine survivalCoroutine;

    void Start()
    {
        SetZoneColor(colorIdle);
    }

    void OnTriggerEnter(Collider other)
    {
        if (completed || !other.CompareTag("Player")) return;

        if (!requireSurvivalTime)
        {
            // Completar inmediatamente
            CompleteZoneMission();
        }
        else
        {
            // Iniciar conteo de supervivencia
            SetZoneColor(colorActive);
            survivalCoroutine = StartCoroutine(SurvivalTimer());
            Debug.Log($"[MissionZone] Jugador en zona '{missionID}'. Sobrevive {survivalSeconds}s...");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (completed || !other.CompareTag("Player")) return;

        if (requireSurvivalTime && survivalCoroutine != null)
        {
            StopCoroutine(survivalCoroutine);
            survivalCoroutine = null;
            SetZoneColor(colorIdle);
            Debug.Log($"[MissionZone] Jugador salió de la zona '{missionID}'. Progreso reiniciado.");
        }
    }

    private IEnumerator SurvivalTimer()
    {
        float elapsed = 0f;
        while (elapsed < survivalSeconds)
        {
            elapsed += Time.deltaTime;
            // Opcional: registrar progreso parcial
            int progressInt = Mathf.FloorToInt((elapsed / survivalSeconds) * 100);
            yield return null;
        }
        CompleteZoneMission();
    }

    private void CompleteZoneMission()
    {
        if (completed) return;
        completed = true;

        MissionManager.Instance?.CompleteMission(missionID);
        SetZoneColor(colorCompleted);
        Debug.Log($"[MissionZone] ✅ Zona de misión '{missionID}' completada.");
    }

    private void SetZoneColor(Color color)
    {
        if (zoneRenderer != null)
            zoneRenderer.material.color = color;
    }

    // Editor visual
    void OnDrawGizmos()
    {
        Gizmos.color = completed ? Color.green : new Color(0.2f, 0.8f, 1f, 0.4f);
        var col = GetComponent<Collider>();
        if (col != null)
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
    }
}
