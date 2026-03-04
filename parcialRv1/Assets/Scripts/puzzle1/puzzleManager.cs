using UnityEngine;
using UnityEngine.SceneManagement;


public class PuzzleManager : MonoBehaviour
{
    public rotacion[] piezas;

    public GameObject panelVictoria;
    public GameObject panelDerrota;

    public bool nivelTerminado = false;

    public void CheckWinCondition()
    {
        if (nivelTerminado) return;

        foreach (rotacion pieza in piezas)
        {
            if (!pieza.isPlaced)
                return;
        }

        Victoria();
    }

    void Victoria()
    {
        nivelTerminado = true;

        panelVictoria.SetActive(true);
        panelDerrota.SetActive(false);

        PausarJuego();
    }

    public void DerrotaPorTiempo()
    {
        if (nivelTerminado) return;

        nivelTerminado = true;

        panelDerrota.SetActive(true);
        panelVictoria.SetActive(false);

        PausarJuego();
    }

    void PausarJuego()
    {
        Time.timeScale = 0f;
    }
    public void Reintentar()
    {
    Time.timeScale = 1f;
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
