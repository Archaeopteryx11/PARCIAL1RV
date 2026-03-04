using UnityEngine;
using TMPro;

public class Reloj : MonoBehaviour
{
    public TextMeshProUGUI contadorText;

    public float tiempoInicial = 120f;
    private float tiempoActual;

    public bool pausado = false;

    public PuzzleManager puzzleManager;

    public Color colorNormal = Color.white;
    public Color colorAdvertencia = Color.yellow;
    public Color colorCritico = Color.red;

    public float tiempoAdvertencia = 30f;
    public float tiempoCritico = 10f;

    void Start()
    {
        
        tiempoActual = tiempoInicial;
        ActualizarUI();
    }

    void Update()
    {
        if (pausado) return;

        tiempoActual -= Time.deltaTime;

        ActualizarUI();

        if (tiempoActual <= 0)
        {
            tiempoActual = 0;
            pausado = true;

            puzzleManager.DerrotaPorTiempo();
            
        }
 
    }


    void ActualizarUI()
    {
        int minutos = Mathf.FloorToInt(tiempoActual / 60);
        int segundos = Mathf.FloorToInt(tiempoActual % 60);

        string v = $"{minutos}:{segundos:00}";
        contadorText.text = v;

        if (tiempoActual <= tiempoCritico)
            contadorText.color = colorCritico;
        else if (tiempoActual <= tiempoAdvertencia)
            contadorText.color = colorAdvertencia;
        else
            contadorText.color = colorNormal;
    }

    public void AlternarPausa()
    {
        pausado = !pausado;
    }
}
