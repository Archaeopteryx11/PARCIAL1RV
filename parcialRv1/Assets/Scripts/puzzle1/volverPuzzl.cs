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
        Debug.Log($"[PuzzleVolver] EscenaAnterior: {PlayerPrefs.GetString("EscenaAnterior", "VACÍO")}");
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

    // Conecta este método al botón de Reintentar del PanelVictoria
    public void VolverEscenaPrincipal()
    {
        Time.timeScale = 1f;
        string escenaAnterior = PlayerPrefs.GetString("EscenaAnterior", "");
        if (!string.IsNullOrEmpty(escenaAnterior))
            SceneManager.LoadScene(escenaAnterior);
        else
            SceneManager.LoadScene(0);
    }
}