using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CapturaManager : MonoBehaviour
{
    [SerializeField] private Button btnCaptura;
    [SerializeField] private GameObject flashPanel;

    void Start()
    {
        btnCaptura.onClick.AddListener(TomarFoto);
    }

    void TomarFoto()
    {
        StartCoroutine(CapturarPantalla());
    }

    IEnumerator CapturarPantalla()
    {
        if (flashPanel != null)
        {
            flashPanel.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            flashPanel.SetActive(false);
        }

        yield return new WaitForEndOfFrame();

        Texture2D captura = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        captura.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        captura.Apply();

        string nombre = "LUMIERE_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        string rutaTemporal = Application.temporaryCachePath + "/" + nombre;
        byte[] bytes = captura.EncodeToPNG();
        System.IO.File.WriteAllBytes(rutaTemporal, bytes);

        Destroy(captura);

        NativeGallery.SaveImageToGallery(
            rutaTemporal,
            "LUMIERE",
            nombre,
            (success, path) =>
            {
                if (success)
                    Debug.Log("Foto guardada: " + path);
                else
                    Debug.Log("Error al guardar: " + path);
            }
        );
    }
}