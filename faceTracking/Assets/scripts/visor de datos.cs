using UnityEngine;
using TMPro;

public class VisorDatos : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoStats;

    void OnEnable()
    {
        if (DataManager.Instance == null) return;

        string info = "";
        info += $"Total usos: {DataManager.Instance.GetTotalUsos()}\n";
        info += $"Usuarios registrados: {DataManager.Instance.GetUsuarios().Count}\n";

        textoStats.text = info;
    }
}