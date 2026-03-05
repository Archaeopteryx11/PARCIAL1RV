using UnityEngine;

/// <summary>
/// MISIÓN 4 — Minijuego 2D de conexión de tuberías
/// Al interactuar, abre un panel 2D donde el jugador
/// rota piezas para conectar la tubería de entrada a la salida.
///
/// SETUP:
/// 1. Crea un GameObject "TuberiaRota" — adjunta PipeMinigameMission.cs
/// 2. Agrega Collider (IsTrigger = false) para el raycast
/// 3. Asigna el panel del minijuego (pipeMinigamePanel) en el Inspector
/// 4. El panel debe tener el script PipeMinigameController.cs
/// </summary>
public class PipeMinigameMission : BaseMission, IInteractable
{
    [Header("Minijuego")]
    [Tooltip("Panel de UI del minijuego 2D (Canvas > PipeMinigamePanel)")]
    public GameObject pipeMinigamePanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip   openSound;

    private MovePlayer currentPlayer = null;
    private bool       minigameOpen  = false;

    protected override void Start()
    {
        missionName        = "Conectar tuberías";
        missionDescription = "Rota las piezas para conectar el flujo de agua";
        base.Start();

        if (pipeMinigamePanel != null)
            pipeMinigamePanel.SetActive(false);
    }

    public void Interact(MovePlayer player)
    {
        if (IsCompleted || minigameOpen) return;

        currentPlayer = player;
        minigameOpen  = true;

        if (pipeMinigamePanel != null)
            pipeMinigamePanel.SetActive(true);

        if (audioSource != null && openSound != null)
            audioSource.PlayOneShot(openSound);

        // Desbloquear cursor para usar el minijuego con mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        Debug.Log($"[PipeMinigame] {player.name} abrió el minijuego de tuberías.");
    }

    /// <summary>
    /// Llamar desde PipeMinigameController cuando el jugador completa el puzzle.
    /// </summary>
    public void OnMinigameCompleted()
    {
        minigameOpen = false;

        if (pipeMinigamePanel != null)
            pipeMinigamePanel.SetActive(false);

        // Restaurar cursor bloqueado
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;

        Complete();
    }

    /// <summary>
    /// Llamar desde PipeMinigameController si el jugador cierra sin completar.
    /// </summary>
    public void OnMinigameClosed()
    {
        minigameOpen  = false;
        currentPlayer = null;

        if (pipeMinigamePanel != null)
            pipeMinigamePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }
}
