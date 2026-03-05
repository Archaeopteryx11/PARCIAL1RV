using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleVolver : MonoBehaviour
{
    public float esperaAntesDeSalir = 2f;

    private PuzzleManager puzzleManager;
    private bool yaEsperandoSalir = false;

    void Start()
    {
        puzzleManager = GetComponent<PuzzleManager>();
    }

    void Update()
    {
        if (yaEsperandoSalir) return;

        if (puzzleManager != null && puzzleManager.nivelTerminado)
        {
            yaEsperandoSalir = true;
            Invoke(nameof(VolverEscenaPrincipal), esperaAntesDeSalir);
        }
    }

    void VolverEscenaPrincipal()
    {
        Time.timeScale = 1f;

        string escenaAnterior = PlayerPrefs.GetString("EscenaAnterior", "");

        if (!string.IsNullOrEmpty(escenaAnterior))
            SceneManager.LoadScene(escenaAnterior);
        else
            SceneManager.LoadScene(0); // Fallback: carga la primera escena
    }
}