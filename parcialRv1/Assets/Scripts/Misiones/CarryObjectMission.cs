using UnityEngine;

/// <summary>
/// MISIÓN 2 — Recoger objeto y llevarlo a un punto
/// Jugador recoge una pieza de tubería y la lleva al punto de entrega.
///
/// SETUP:
/// 1. Crea un GameObject "PiezaTuberia" — adjunta CarryObjectMission.cs
/// 2. Agrega Collider al objeto (IsTrigger = false) para el raycast
/// 3. Crea un GameObject "PuntoEntrega" — adjunta el script DeliveryPoint.cs
///    o simplemente crea un trigger y asígnalo al campo deliveryPoint
/// 4. Asigna deliveryPoint en el Inspector
/// </summary>
public class CarryObjectMission : BaseMission, IInteractable
{
    [Header("Entrega")]
    [Tooltip("El trigger donde el jugador debe llevar el objeto")]
    public Transform deliveryPoint;
    [Tooltip("Radio del punto de entrega para detectar llegada")]
    public float deliveryRadius = 1.5f;

    [Header("Visual")]
    [Tooltip("El objeto visual que el jugador 'carga' (se desactiva al recoger)")]
    public GameObject objectVisual;
    [Tooltip("Indicador visual del punto de entrega (parpadea)")]
    public GameObject deliveryIndicator;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip   pickupSound;
    public AudioClip   deliverSound;

    private MovePlayer carrier      = null; // Jugador que lleva el objeto
    private bool       isCarried    = false;

    protected override void Start()
    {
        missionName        = "Entregar pieza";
        missionDescription = "Recoge la pieza de tubería y llévala al punto marcado";
        base.Start();

        if (deliveryIndicator != null)
            deliveryIndicator.SetActive(true);
    }

    // Jugador interactúa con el objeto para recogerlo
    public void Interact(MovePlayer player)
    {
        if (IsCompleted) return;

        if (!isCarried)
        {
            // Recoger
            isCarried      = true;
            carrier        = player;

            if (objectVisual != null) objectVisual.SetActive(false);
            if (audioSource  != null && pickupSound != null)
                audioSource.PlayOneShot(pickupSound);

            Debug.Log($"[CarryMission] {player.name} recogió la pieza.");
        }
        else if (carrier == player)
        {
            // Soltar (cancelar)
            isCarried = false;
            carrier   = null;
            if (objectVisual != null) objectVisual.SetActive(true);
            Debug.Log($"[CarryMission] {player.name} soltó la pieza.");
        }
    }

    void Update()
    {
        if (!isCarried || IsCompleted || carrier == null) return;

        // Verificar si el portador llegó al punto de entrega
        if (deliveryPoint != null)
        {
            float dist = Vector3.Distance(carrier.transform.position, deliveryPoint.position);
            if (dist <= deliveryRadius)
                Deliver();
        }
    }

    private void Deliver()
    {
        isCarried = false;

        if (deliveryIndicator != null) deliveryIndicator.SetActive(false);
        if (audioSource != null && deliverSound != null)
            audioSource.PlayOneShot(deliverSound);

        Debug.Log("[CarryMission] ¡Pieza entregada!");
        Complete();
    }

    // Gizmo para ver el radio de entrega en el editor
    void OnDrawGizmosSelected()
    {
        if (deliveryPoint == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(deliveryPoint.position, deliveryRadius);
    }
}
