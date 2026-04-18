using UnityEngine;
using UnityEngine.Events;

public class BotonSecreto : MonoBehaviour
{
    [SerializeField] private int clicsNecesarios = 5;
    [SerializeField] private float tiempoMaximoEntreClics = 0.5f;
    [SerializeField] private GameObject panelStats; // El objeto que tiene tu script VisorDatos

    private int contadorClics = 0;
    private float tiempoUltimoClic = 0f;

    public void RegistrarClic()
    {
        float tiempoActual = Time.time;

        // Si pasa mucho tiempo entre clics, se reinicia el contador
        if (tiempoActual - tiempoUltimoClic > tiempoMaximoEntreClics)
        {
            contadorClics = 0;
        }

        contadorClics++;
        tiempoUltimoClic = tiempoActual;

        if (contadorClics >= clicsNecesarios)
        {
            MostrarPanel();
            contadorClics = 0; // Reiniciamos para que se pueda volver a usar
        }
    }

    private void MostrarPanel()
    {
        if (panelStats != null)
        {
            panelStats.SetActive(true);
        }
    }
}