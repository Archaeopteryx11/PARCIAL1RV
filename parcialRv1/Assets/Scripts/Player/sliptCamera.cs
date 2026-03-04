using UnityEngine;

public class SplitScreenManager : MonoBehaviour
{
    [Header("Cámaras de los jugadores (en orden: 0, 1, 2, 3)")]
    public Camera[] camarasJugadores = new Camera[4];

    private static readonly Rect[] viewports = new Rect[]
    {
        new Rect(0f,   0.5f,  0.5f, 0.5f),   // Player 0 — arriba izquierda
        new Rect(0.5f, 0.5f,  0.5f, 0.5f),   // Player 1 — arriba derecha
        new Rect(0f,   0f,    0.5f, 0.5f),    // Player 2 — abajo izquierda
        new Rect(0.5f, 0f,    0.5f, 0.5f),    // Player 3 — abajo derecha
    };

    void Awake()
    {
        AcomodarCamaras();
    }

    void AcomodarCamaras()
    {
        for (int i = 0; i < camarasJugadores.Length; i++)
        {
            if (camarasJugadores[i] == null)
            {
                Debug.LogWarning($"SplitScreenManager: falta asignar la cámara del Player {i}");    
                continue;
            }

            camarasJugadores[i].rect = viewports[i];
        }

        Debug.Log("SplitScreenManager: cámaras acomodadas en grilla 2x2 ?");
    }

#if UNITY_EDITOR
    // Botón en el Inspector para previsualizar sin correr el juego
    [ContextMenu("Previsualizar en Editor")]
    void PrevisualizarEnEditor()
    {
        AcomodarCamaras();
    }
#endif
}