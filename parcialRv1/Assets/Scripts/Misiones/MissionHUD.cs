using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Muestra el estado de las 5 misiones en pantalla.
/// Se actualiza automáticamente cuando una misión se completa.
///
/// SETUP:
/// 1. En el Canvas crea un panel "MissionHUD"
/// 2. Adjunta MissionHUD.cs al panel
/// 3. Crea 5 filas de UI con: ícono, nombre y checkmark
///    Asígnalas a los arrays en el Inspector
/// </summary>
public class MissionHUD : MonoBehaviour
{
    [System.Serializable]
    public class MissionRow
    {
        public TextMeshProUGUI nameText;
        public Image           checkmark;      // ✓ que aparece al completar
        public Image           background;     // Fondo que cambia de color
    }

    [Header("Filas de misiones (una por misión, en orden)")]
    public MissionRow[] rows = new MissionRow[5];

    [Header("Colores")]
    public Color colorPending   = new Color(1f, 1f, 1f, 0.5f);
    public Color colorCompleted = new Color(0.3f, 1f, 0.5f, 1f);
    public Color bgPending      = new Color(0f, 0f, 0f, 0.4f);
    public Color bgCompleted    = new Color(0.1f, 0.4f, 0.2f, 0.5f);

    [Header("Contador")]
    public TextMeshProUGUI counterText; // "2 / 5 misiones"

    private List<BaseMission> missions;
    private bool initialized = false;

    void Update()
    {
        if (!initialized) TryInit();
    }

    private void TryInit()
    {
        if (MissionManager.Instance == null) return;

        missions = MissionManager.Instance.GetAllMissions();
        if (missions.Count == 0) return;

        // Inicializar textos
        for (int i = 0; i < rows.Length && i < missions.Count; i++)
        {
            if (rows[i].nameText  != null) rows[i].nameText.text  = missions[i].missionName;
            if (rows[i].checkmark != null) rows[i].checkmark.gameObject.SetActive(false);
            if (rows[i].background!= null) rows[i].background.color = bgPending;
        }

        // Suscribirse al evento de misión completada
        MissionManager.Instance.OnMissionCompleted.AddListener(OnMissionCompleted);

        initialized = true;
        UpdateCounter();
    }

    private void OnMissionCompleted(BaseMission mission)
    {
        if (missions == null) return;

        int index = missions.IndexOf(mission);
        if (index < 0 || index >= rows.Length) return;

        // Actualizar la fila de esa misión
        if (rows[index].nameText  != null) rows[index].nameText.color = colorCompleted;
        if (rows[index].checkmark != null) rows[index].checkmark.gameObject.SetActive(true);
        if (rows[index].background!= null) rows[index].background.color = bgCompleted;

        UpdateCounter();
    }

    private void UpdateCounter()
    {
        if (counterText == null || MissionManager.Instance == null) return;
        int total     = MissionManager.Instance.GetAllMissions().Count;
        int completed = MissionManager.Instance.CompletedCount();
        counterText.text = $"{completed} / {total} misiones";
    }

    void OnDestroy()
    {
        if (MissionManager.Instance != null)
            MissionManager.Instance.OnMissionCompleted.RemoveListener(OnMissionCompleted);
    }
}
