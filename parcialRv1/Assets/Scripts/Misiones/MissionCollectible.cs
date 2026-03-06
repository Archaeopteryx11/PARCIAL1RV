//using UnityEngine;

///// <summary>
///// Objeto coleccionable que registra progreso en una misión al ser recogido.
///// Adjunta a cualquier objeto del mapa que el jugador deba recoger.
/////
///// SETUP en Unity:
///// 1. Adjunta a un GameObject con Collider (marcar Is Trigger = true).
///// 2. Llena el missionID con el mismo ID del MissionData correspondiente.
///// 3. El jugador debe tener tag "Player".
///// </summary>
//public class MissionCollectible : MonoBehaviour
//{
//    [Header("Misión")]
//    [Tooltip("Debe coincidir exactamente con el missionID del ScriptableObject")]
//    public string missionID;

//    [Header("Efectos al Recoger")]
//    [Tooltip("Objeto de efecto visual (partículas, destello, etc.)")]
//    public GameObject collectEffect;
//    [Tooltip("Sonido al recoger (AudioSource en este mismo objeto)")]
//    public AudioSource collectSound;

//    [Header("Comportamiento")]
//    [Tooltip("¿El objeto desaparece al recogerse?")]
//    public bool destroyOnCollect = true;
//    [Tooltip("¿Cuánto progreso aporta esta recogida?")]
//    public int progressAmount = 1;

//    private bool collected = false;

//    void OnTriggerEnter(Collider other)
//    {
//        if (collected) return;
//        if (!other.CompareTag("Player")) return;

//        Collect();
//    }

//    public void Collect()
//    {
//        if (collected) return;
//        collected = true;

//        // Registrar progreso en el MissionManager
//        if (MissionManager.Instance != null)
//            MissionManager.Instance.RegisterProgress(missionID, progressAmount);
//        else
//            Debug.LogWarning("[MissionCollectible] No se encontró MissionManager en la escena.");

//        // Efectos visuales y de audio
//        if (collectEffect != null)
//            Instantiate(collectEffect, transform.position, Quaternion.identity);

//        if (collectSound != null)
//        {
//            collectSound.transform.parent = null; // Desanclar para que el sonido no se corte
//            collectSound.Play();
//            Destroy(collectSound.gameObject, collectSound.clip.length + 0.1f);
//        }

//        if (destroyOnCollect)
//            Destroy(gameObject);
//        else
//            gameObject.SetActive(false);
//    }
//}


using UnityEngine;

/// <summary>
/// Objeto coleccionable: al tocarlo el jugador desaparece y registra progreso.
///
/// SETUP en Unity:
/// 1. Adjunta a un GameObject con Collider (Is Trigger = true).
/// 2. Pon el missionID igual al del ScriptableObject correspondiente.
/// 3. El jugador debe tener tag "Player".
/// </summary>
public class MissionCollectible : MonoBehaviour
{
    [Header("Misión")]
    [Tooltip("Debe coincidir exactamente con el missionID del ScriptableObject")]
    public string missionID;

    [Tooltip("Cuánto progreso aporta esta recogida")]
    public int progressAmount = 1;

    private bool collected = false;

    void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;

        // Registrar progreso en el MissionManager
        MissionManager.Instance?.RegisterProgress(missionID, progressAmount);

        // Desaparecer inmediatamente
        gameObject.SetActive(false);
    }
}