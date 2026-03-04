using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// MISIÓN 3 — Reparar fuga (2 jugadores simultáneos)
/// Dos jugadores deben presionar Interact al mismo tiempo
/// en dos puntos distintos de la tubería.
///
/// SETUP:
/// 1. Crea un GameObject "FugaTuberia" — adjunta CoopRepairMission.cs
/// 2. Crea 2 hijos: "PuntoA" y "PuntoB" — cada uno con Collider y CoopPoint.cs
/// 3. Asigna repairPointA y repairPointB en el Inspector
/// </summary>
public class CoopRepairMission : BaseMission
{
    [Header("Puntos de reparación")]
    [Tooltip("Primer punto — un jugador debe pararse aquí")]
    public CoopPoint repairPointA;
    [Tooltip("Segundo punto — otro jugador diferente")]
    public CoopPoint repairPointB;

    [Tooltip("Segundos que ambos jugadores deben mantener presionado")]
    public float holdTime = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip   repairSound;
    public AudioClip   completeSound;

    // Temporizador de reparación
    private float repairProgress = 0f;
    private bool  isRepairing    = false;

    protected override void Start()
    {
        missionName        = "Reparar fuga cooperativa";
        missionDescription = "Dos jugadores deben sellar la fuga al mismo tiempo";
        base.Start();

        // Registrar esta misión en ambos puntos
        if (repairPointA != null) repairPointA.Setup(this);
        if (repairPointB != null) repairPointB.Setup(this);
    }

    void Update()
    {
        if (IsCompleted) return;

        bool bothActive = repairPointA != null && repairPointA.IsOccupied &&
                          repairPointB != null && repairPointB.IsOccupied &&
                          repairPointA.CurrentPlayer != repairPointB.CurrentPlayer; // jugadores distintos

        if (bothActive)
        {
            isRepairing    = true;
            repairProgress += Time.deltaTime;

            if (audioSource != null && repairSound != null && !audioSource.isPlaying)
                audioSource.PlayOneShot(repairSound);

            if (repairProgress >= holdTime)
            {
                if (audioSource != null && completeSound != null)
                    audioSource.PlayOneShot(completeSound);
                Complete();
            }
        }
        else
        {
            // Si dejan de mantener, reinicia el progreso
            if (isRepairing)
            {
                isRepairing    = false;
                repairProgress = 0f;
                if (audioSource != null) audioSource.Stop();
            }
        }
    }

    // Propiedad para que el HUD muestre el progreso
    public float RepairProgress => repairProgress / holdTime;

    protected override void OnCompleted()
    {
        if (repairPointA != null) repairPointA.gameObject.SetActive(false);
        if (repairPointB != null) repairPointB.gameObject.SetActive(false);
    }
}


