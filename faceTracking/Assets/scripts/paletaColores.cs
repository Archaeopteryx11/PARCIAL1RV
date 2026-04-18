using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ColorPaleta : MonoBehaviour
{
    public static ColorPaleta Instance;

    [Header("UI")]
    [SerializeField] private GameObject panelPaleta;
    [SerializeField] private Transform contentColores;
    [SerializeField] private GameObject botonColorPrefab;

    private string modeloActual;
    private ARFaceManager faceManager;

    void Awake()
    {
        Instance = this;
        panelPaleta.SetActive(false);
    }

    void Start()
    {
        faceManager = FindFirstObjectByType<ARFaceManager>();
    }

    public void MostrarPaleta(string nombreModelo, Color[] colores)
    {
        if (colores == null || colores.Length == 0)
        {
            OcultarPaleta();
            return;
        }

        // Cancela cualquier animación pendiente y resetea posición
        panelPaleta.GetComponent<RectTransform>().DOKill();
        panelPaleta.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 520);
        panelPaleta.SetActive(true); // Reactiva antes de generar botones

        modeloActual = nombreModelo;

        foreach (Transform child in contentColores)
            Destroy(child.gameObject);

        foreach (var color in colores)
        {
            Color c = color;
            GameObject btn = Instantiate(botonColorPrefab, contentColores);
            var img = btn.GetComponent<Image>();
            if (img != null) img.color = c;

            btn.GetComponent<Button>().onClick.AddListener(
                () => AplicarColor(c, btn)
            );
        }
    }

    public void OcultarPaleta()
    {
        panelPaleta.GetComponent<RectTransform>()
                   .DOAnchorPosY(-200, 0.2f)
                   .SetEase(Ease.InBack)
                   .OnComplete(() => panelPaleta.SetActive(false));
    }

    void AplicarColor(Color color, GameObject btnSeleccionado)
    {
        // Quita borde de todos los botones
        foreach (Transform child in contentColores)
        {
            var borde = child.Find("Borde")?.GetComponent<Image>();
            if (borde != null) borde.color = new Color(0.788f, 0.659f, 0.298f, 0f);
        }

        // Activa borde del seleccionado
        var bordeActivo = btnSeleccionado.transform.Find("Borde")?.GetComponent<Image>();
        if (bordeActivo != null)
            bordeActivo.color = new Color(0.788f, 0.659f, 0.298f, 1f);

        // Aplica color al modelo
        foreach (var face in faceManager.trackables)
        {
            var modelo = face.transform.Find(modeloActual);
            if (modelo == null) continue;
            var renderers = modelo.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                var mats = r.materials;
                foreach (var m in mats)
                    m.color = color;
                r.materials = mats;
            }
        }
    }
}