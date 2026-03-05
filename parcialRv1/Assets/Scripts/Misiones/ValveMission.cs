using UnityEngine;

/// <summary>
/// MISIÓN 1 — Activar válvula
/// El jugador se acerca y presiona Interact (E / Botón Sur)
/// para girar la válvula y completar la misión.
///
/// SETUP:
/// 1. Crea un GameObject en la escena con la geometría de la válvula
/// 2. Adjunta ValveMission.cs
/// 3. Agrega un Collider (IsTrigger = false) para que el raycast lo detecte
/// 4. Asigna el nombre de la misión en el Inspector
/// </summary>
public class ValveMission : BaseMission, IInteractable
{
    [Header("Válvula")]
    [Tooltip("Parte visual que rota al activar (la rueda de la válvula)")]
    public Transform valveWheel;
    [Tooltip("Cuántos grados gira la rueda al activarse")]
    public float rotationAmount = 180f;
    [Tooltip("Velocidad de la animación de giro")]
    public float rotationSpeed  = 90f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip   valveSound;

    private bool isActivating = false;
    private float rotated     = 0f;

    protected override void Start()
    {
        missionName        = "Activar válvula";
        missionDescription = "Gira la válvula de cierre de emergencia";
        base.Start();
    }

    // Llamado por MovePlayer.TryInteract() cuando el jugador mira la válvula y presiona E
    public void Interact(MovePlayer player)
    {
        if (IsCompleted || isActivating) return;
        isActivating = true;

        if (audioSource != null && valveSound != null)
            audioSource.PlayOneShot(valveSound);

        Debug.Log($"[ValveMission] Jugador {player.name} activó la válvula.");
    }

    void Update()
    {
        if (!isActivating || IsCompleted) return;

        // Animar la rueda girando
        float step = rotationSpeed * Time.deltaTime;
        valveWheel?.Rotate(Vector3.forward * step);
        rotated += step;

        if (rotated >= rotationAmount)
        {
            isActivating = false;
            Complete();
        }
    }

    protected override void OnCompleted()
    {
        // Efecto visual: cambiar color a verde para indicar completada
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
            rend.material.color = new Color(0.2f, 0.8f, 0.3f);
    }
}
