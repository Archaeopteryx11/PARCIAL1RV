using UnityEngine;

/// <summary>
/// MISIÓN 5 — Minijuego 2D de soldadura
/// El jugador debe mantener el cursor sobre una línea de soldadura
/// sin salirse del camino durante un tiempo determinado.
///
/// SETUP:
/// 1. Crea un GameObject "PuntoSoldadura" — adjunta WeldingMinigameMission.cs
/// 2. Agrega Collider (IsTrigger = false) para el raycast
/// 3. Asigna el panel del minijuego (weldingMinigamePanel) en el Inspector
/// 4. El panel debe tener el script WeldingMinigameController.cs
/// </summary>
public class WeldingMinigameMission : BaseMission, IInteractable
{
    [Header("Minijuego")]
    [Tooltip("Panel de UI del minijuego 2D de soldadura")]
    public GameObject weldingMinigamePanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip   openSound;
    public AudioClip   weldingLoopSound; // Sonido continuo mientras se suelda

    private bool minigameOpen = false;

    protected override void Start()
    {
        missionName        = "Soldar tubería";
        missionDescription = "Mantén el pulso firme para soldar el tramo dañado";
        base.Start();

        if (weldingMinigamePanel != null)
            weldingMinigamePanel.SetActive(false);
    }

    public void Interact(MovePlayer player)
    {
        if (IsCompleted || minigameOpen) return;

        minigameOpen = true;

        if (weldingMinigamePanel != null)
            weldingMinigamePanel.SetActive(true);

        if (audioSource != null && openSound != null)
            audioSource.PlayOneShot(openSound);

        // Desbloquear cursor para el minijuego
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        Debug.Log($"[WeldingMinigame] {player.name} abrió el minijuego de soldadura.");
    }

    /// <summary>
    /// Llamar desde WeldingMinigameController cuando el jugador completa la soldadura.
    /// </summary>
    public void OnWeldingCompleted()
    {
        minigameOpen = false;

        if (weldingMinigamePanel != null)
            weldingMinigamePanel.SetActive(false);

        if (audioSource != null) audioSource.Stop();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        Complete();
    }

    /// <summary>
    /// Llamar si el jugador falla o cierra el minijuego sin completarlo.
    /// </summary>
    public void OnWeldingFailed()
    {
        minigameOpen = false;

        if (weldingMinigamePanel != null)
            weldingMinigamePanel.SetActive(false);

        if (audioSource != null) audioSource.Stop();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        // No llama Complete() — el jugador puede intentarlo de nuevo
        Debug.Log("[WeldingMinigame] Soldadura fallida, intenta de nuevo.");
    }
}
