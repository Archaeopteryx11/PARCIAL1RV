using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelRegistro : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputNombre;
    [SerializeField] private TMP_InputField inputCorreo;
    [SerializeField] private Button btnEntrar;
    [SerializeField] private TextMeshProUGUI textoError;

    [SerializeField] private GameObject panelContenedor;

    void Start()
    {

        // Opcional: Limpiar los campos por si quedaron escritos de la sesión anterior
        if (inputNombre != null) inputNombre.text = "";
        if (inputCorreo != null) inputCorreo.text = "";
        if (textoError != null) textoError.text = "";
    }

    void OnEnable()
    {
        btnEntrar.onClick.AddListener(Registrar);
    }

    void OnDisable()
    {
        btnEntrar.onClick.RemoveListener(Registrar);
    }

    void Registrar()
    {
        string nombre = inputNombre.text.Trim();
        string correo = inputCorreo.text.Trim();
        Debug.Log($"Nombre: '{nombre}' Correo: '{correo}'");

        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(correo))
        {
            textoError.text = "Por favor completa todos los campos";
            Debug.Log("Campos vacios");
            return;
        }

        if (!correo.Contains("@"))
        {
            textoError.text = "Correo no valido";
            Debug.Log("Correo invalido");
            return;
        }

        // Intenta registrar, pero no importa si ya existe
        DataManager.Instance.RegistrarUsuario(nombre, correo);

        // Siempre oculta el panel si los datos son válidos
        panelContenedor.SetActive(false);
    }
}