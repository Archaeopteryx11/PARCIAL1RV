using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PuzzleTrigger : MonoBehaviour
{
    //nombre de la escena puzzle
    public string nombreEscenaPuzzle;

    //mensaje para el personje que se acerca
    public string mensajeInteraccion = "Interactuar";

    private bool jugadorCerca = false;
    private PlayerInput playerInput;

    // UI
    private GUIStyle estiloTexto;

    void Update()
    {
        if (!jugadorCerca || playerInput == null) return;

        // Detectar E en teclado o botón Sur en gamepad 
        var keyboard = Keyboard.current;
        var gamepad = Gamepad.current;

        bool presionoInteractuar =
            (keyboard != null && keyboard.eKey.wasPressedThisFrame) ||
            (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame);

        if (presionoInteractuar)
        {
            // Guardar la escena actual para volver después
            PlayerPrefs.SetString("EscenaAnterior", SceneManager.GetActiveScene().name);
            PlayerPrefs.Save();

            SceneManager.LoadScene(nombreEscenaPuzzle);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerInput pi = other.GetComponent<PlayerInput>();
        if (pi != null)
        {
            jugadorCerca = true;
            playerInput = pi;
        }
    }

    void OnTriggerExit(Collider other)
    {
        PlayerInput pi = other.GetComponent<PlayerInput>();
        if (pi != null)
        {
            jugadorCerca = false;
            playerInput = null;
        }
    }

    // Mostrar mensaje en pantalla cuando el jugador está cerca
    void OnGUI()
    {
        if (!jugadorCerca) return;

        GUIStyle estilo = new GUIStyle(GUI.skin.label);
        estilo.fontSize = 22;
        estilo.fontStyle = FontStyle.Bold;
        estilo.normal.textColor = Color.white;
        estilo.alignment = TextAnchor.MiddleCenter;

        float w = 500f, h = 40f;
        Rect rect = new Rect((Screen.width - w) / 2f, Screen.height - 120f, w, h);
        GUI.Label(rect, mensajeInteraccion, estilo);
    }
}