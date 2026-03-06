using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// HUD que muestra la lista de misiones activas con su progreso.
/// Se actualiza en tiempo real y resalta las misiones al completarse.
///
/// SETUP en Unity:
/// 1. Adjunta a un GameObject dentro de tu Canvas.
/// 2. Crea un prefab MissionEntry (Panel con: TextTitle, TextProgress, Slider, IconImage).
/// 3. Crea un contenedor vacío (Vertical Layout Group) y asígnalo a "entriesContainer".
/// 4. Asigna el prefab a "missionEntryPrefab".
/// </summary>
public class MissionHUD : MonoBehaviour
{
    [Header("Referencias")]
    public MissionManager missionManager;

    [Header("UI - Contenedor")]
    [Tooltip("Transform con Vertical Layout Group donde aparecen las entradas")]
    public Transform entriesContainer;
    [Tooltip("Prefab de una entrada de misión (ver estructura abajo)")]
    public GameObject missionEntryPrefab;

    [Header("Colores")]
    public Color colorActive = Color.white;
    public Color colorCompleted = new Color(0.3f, 1f, 0.4f);

    [Header("Mensaje de Victoria")]
    [Tooltip("Panel que aparece al completar todas las misiones")]
    public GameObject allMissionsCompletePanel;

    // Diccionario ID → entrada de UI instanciada
    private Dictionary<string, MissionEntryUI> uiEntries = new Dictionary<string, MissionEntryUI>();

    void Start()
    {
        if (missionManager == null)
            missionManager = MissionManager.Instance;

        // Suscribirse a eventos del MissionManager
        missionManager.OnMissionStarted.AddListener(OnMissionStarted);
        missionManager.OnMissionCompleted.AddListener(OnMissionCompleted);
        missionManager.OnAllMissionsCompleted.AddListener(OnAllMissionsCompleted);

        if (allMissionsCompletePanel != null)
            allMissionsCompletePanel.SetActive(false);
    }

    void Update()
    {
        // Actualizar progreso de misiones activas cada frame
        foreach (var pair in uiEntries)
        {
            var inst = missionManager.GetMission(pair.Key);
            if (inst != null && inst.status == MissionStatus.Active)
                pair.Value.UpdateProgress(inst.Progress, inst.currentCount, inst.data.requiredCount);
        }
    }

    // ── Callbacks de eventos ──────────────────────────────────────

    private void OnMissionStarted(MissionData data)
    {
        if (missionEntryPrefab == null || entriesContainer == null) return;

        var go = Instantiate(missionEntryPrefab, entriesContainer);
        var entry = go.GetComponent<MissionEntryUI>();

        if (entry != null)
        {
            entry.Initialize(data, colorActive);
            uiEntries[data.missionID] = entry;
        }
    }

    private void OnMissionCompleted(MissionData data)
    {
        if (uiEntries.TryGetValue(data.missionID, out var entry))
            entry.SetCompleted(colorCompleted);
    }

    private void OnAllMissionsCompleted()
    {
        if (allMissionsCompletePanel != null)
            allMissionsCompletePanel.SetActive(true);
    }
}

/// <summary>
/// Componente que va dentro del prefab de cada entrada de misión.
/// Asigna los campos desde el Inspector del prefab.
/// </summary>
public class MissionEntryUI : MonoBehaviour
{
    [Header("Elementos del Prefab")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI progressText;
    public Slider progressBar;
    public Image iconImage;
    public Image backgroundImage;

    public void Initialize(MissionData data, Color color)
    {
        if (titleText != null) titleText.text = data.missionTitle;
        if (progressText != null) progressText.text = $"0 / {data.requiredCount}";
        if (progressBar != null) { progressBar.minValue = 0; progressBar.maxValue = 1; progressBar.value = 0; }
        if (iconImage != null && data.icon != null) iconImage.sprite = data.icon;
        if (backgroundImage != null) backgroundImage.color = color;
    }

    public void UpdateProgress(float progress, int current, int required)
    {
        if (progressBar != null) progressBar.value = progress;
        if (progressText != null) progressText.text = $"{current} / {required}";
    }

    public void SetCompleted(Color completedColor)
    {
        if (titleText != null) titleText.text = "✅ " + titleText.text;
        if (progressBar != null) progressBar.value = 1f;
        if (progressText != null) progressText.text = "¡Completada!";
        if (backgroundImage != null) backgroundImage.color = completedColor;
    }
}
