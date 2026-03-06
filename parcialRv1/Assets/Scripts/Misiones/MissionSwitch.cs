using UnityEngine;

/// <summary>
/// Palanca / botón interactuable que registra progreso en una misión.
/// El jugador la activa al presionar una tecla dentro del rango de interacción.
///
/// SETUP en Unity:
/// 1. Adjunta a un GameObject (palanca, botón, panel...).
/// 2. Agrega un Collider (Is Trigger = true) como zona de detección.
/// 3. El jugador debe tener tag "Player".
/// 4. Asigna el missionID correspondiente.
/// </summary>
public class MissionSwitch : MonoBehaviour
{
    [Header("Misión")]
    public string missionID;

    [Header("Interacción")]
    [Tooltip("Tecla que activa la palanca")]
    public KeyCode interactKey = KeyCode.E;
    [Tooltip("Animador (opcional) para animar la palanca al activarse)")]
    public Animator switchAnimator;
    [Tooltip("Parámetro bool del Animator que se activa")]
    public string animatorParam = "Activated";

    [Header("Efectos")]
    public GameObject activateEffect;
    public AudioSource activateSound;
    public Light switchLight;
    public Color activatedColor = Color.green;

    private bool playerInRange = false;
    private bool activated = false;

    void Update()
    {
        if (playerInRange && !activated && Input.GetKeyDown(interactKey))
            Activate();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // Aquí podrías mostrar un prompt de "Presiona E para interactuar"
            Debug.Log($"[MissionSwitch] Jugador cerca de: {gameObject.name}. Presiona {interactKey}.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    public void Activate()
    {
        if (activated) return;
        activated = true;

        // Registrar progreso
        MissionManager.Instance?.RegisterProgress(missionID, 1);

        // Animación
        if (switchAnimator != null)
            switchAnimator.SetBool(animatorParam, true);

        // Luz de confirmación
        if (switchLight != null)
            switchLight.color = activatedColor;

        // Efectos
        if (activateEffect != null)
            Instantiate(activateEffect, transform.position, Quaternion.identity);

        if (activateSound != null)
            activateSound.Play();

        Debug.Log($"[MissionSwitch] Palanca activada: {gameObject.name}");
    }

    // Indicador visual en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = activated ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
