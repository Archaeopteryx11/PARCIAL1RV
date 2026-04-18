using UnityEngine;
using System.Collections.Generic;
using System.IO;
public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    [System.Serializable]
    public class Usuario
    {
        public string nombre;
        public string correo;
        public string fechaRegistro;
    }
    [System.Serializable]
    public class DatosApp
    {
        public int totalUsos = 0;
        public List<string> filtrosUsados = new List<string>();
        public List<Usuario> usuarios = new List<Usuario>();
    }
    private DatosApp datos;
    private string rutaArchivo;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            rutaArchivo = Application.persistentDataPath + "/lumiere_data.json";
            CargarDatos();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void CargarDatos()
    {
        if (File.Exists(rutaArchivo))
        {
            string json = File.ReadAllText(rutaArchivo);
            datos = JsonUtility.FromJson<DatosApp>(json);
        }
        else
        {
            datos = new DatosApp();
        }
    }
    void GuardarDatos()
    {
        string json = JsonUtility.ToJson(datos, true);
        File.WriteAllText(rutaArchivo, json);
    }
    // Registra un uso de la app
    public void RegistrarUsoApp()
    {
        datos.totalUsos++;
        GuardarDatos();
        Debug.Log("Total usos: " + datos.totalUsos);
    }
    // Registra un usuario nuevo
    public bool RegistrarUsuario(string nombre, string correo)
    {
        // Verifica si el correo ya existe
        foreach (var u in datos.usuarios)
        {
            if (u.correo == correo)
            {
                Debug.Log("Usuario ya existe");
                return false;
            }
        }
        var nuevoUsuario = new Usuario
        {
            nombre = nombre,
            correo = correo,
            fechaRegistro = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        };
        datos.usuarios.Add(nuevoUsuario);
        GuardarDatos();
        Debug.Log("Usuario registrado: " + nombre);
        return true;
    }
    // Getters para mostrar estadísticas
    public int GetTotalUsos() => datos.totalUsos;
    public List<Usuario> GetUsuarios() => datos.usuarios;
   
}

