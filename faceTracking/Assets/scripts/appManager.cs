using UnityEngine;

public class AppManager : MonoBehaviour
{
    [SerializeField] private GameObject panelRegistro;

    void Start()
    {
        // Este script siempre está activo, así que Start() siempre corre
        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager no encontrado en la escena");
            return;
        }
        panelRegistro.SetActive(true);
    }
}