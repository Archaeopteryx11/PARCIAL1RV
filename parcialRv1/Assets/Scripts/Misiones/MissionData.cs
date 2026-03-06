using UnityEngine;

/// <summary>
/// ScriptableObject que define los datos de una misión.
/// Crea una desde: Assets > Create > Missions > MissionData
/// </summary>
[CreateAssetMenu(fileName = "NewMission", menuName = "Missions/MissionData")]
public class MissionData : ScriptableObject
{
    [Header("Identidad")]
    public string missionID;        // ID único, ej: "mission_cables"
    public string missionTitle;     // Nombre corto mostrado en HUD
    [TextArea(2, 4)]
    public string description;      // Descripción completa para el jugador

    [Header("Tipo de Misión")]
    public MissionType type;

    [Header("Objetivo")]
    [Tooltip("Cuántas veces hay que hacer la acción (recoger X objetos, activar X palancas...)")]
    public int requiredCount = 1;

    [Header("Recompensa")]
    [Tooltip("¿Reduce el nivel del agua al completarse?")]
    public bool reducesWater = false;
    [Tooltip("Cuánto baja el agua al completarse (en unidades Unity)")]
    public float waterReductionAmount = 0.5f;

    [Header("Icono")]
    public Sprite icon;
}

public enum MissionType
{
    CollectItems,       // Recoger objetos del mapa
    ActivateSwitches,   // Activar palancas / botones
    ReachZone,          // Llegar a una zona específica
    SurviveTime,        // Sobrevivir X segundos
    DefeatEnemies,      // Eliminar enemigos
    Custom              // Lógica personalizada via script
}
