using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class FiltroCarrusel : MonoBehaviour
{
    [System.Serializable]
    public class FiltroData
    {
        public string nombre;
        public Sprite preview;
        public string nombreModelo;
        public Material materialMaquillaje;
        public Categoria categoria;
        public Color[] coloresDisponibles;
    }

    public enum Categoria { Maquillaje, Joyeria, Accesorios }

    [Header("Carrusel UI")]
    [SerializeField] private Transform content;
    [SerializeField] private GameObject botonPrefab;
    [SerializeField] private RectTransform carruselPanel;

    [Header("Tabs")]
    [SerializeField] private Button tabMaquillaje;
    [SerializeField] private Button tabJoyeria;
    [SerializeField] private Button tabAccesorios;

    [Header("Colores tabs")]
    [SerializeField] private Color colorActivo = new Color(0.788f, 0.659f, 0.298f);
    [SerializeField] private Color colorInactivo = new Color(0.267f, 0.267f, 0.267f);

    [Header("Filtros")]
    [SerializeField] private List<FiltroData> filtros;

    [Header("AR")]
    [SerializeField] private ARFaceManager faceManager;

    // Ahora guardamos TODOS los índices activos
    private HashSet<int> filtrosActivos = new HashSet<int>();
    private Categoria categoriaActiva = Categoria.Maquillaje;

    void Start()
    {
        tabMaquillaje.transform.parent
                 .GetComponent<RectTransform>()
                 .localScale = Vector3.one;

        if (faceManager == null)
            faceManager = FindFirstObjectByType<ARFaceManager>();

        tabMaquillaje.onClick.AddListener(() => CambiarCategoria(Categoria.Maquillaje));
        tabJoyeria.onClick.AddListener(() => CambiarCategoria(Categoria.Joyeria));
        tabAccesorios.onClick.AddListener(() => CambiarCategoria(Categoria.Accesorios));

        CambiarCategoria(Categoria.Maquillaje);
    }



    void CambiarCategoria(Categoria nueva)
    {
        categoriaActiva = nueva;
        ActualizarColorTabs();
        GenerarBotones();
    }

    void ActualizarColorTabs()
    {
        SetColorTab(tabMaquillaje, categoriaActiva == Categoria.Maquillaje);
        SetColorTab(tabJoyeria, categoriaActiva == Categoria.Joyeria);
        SetColorTab(tabAccesorios, categoriaActiva == Categoria.Accesorios);
    }

    void SetColorTab(Button tab, bool activo)
    {
        var texto = tab.GetComponentInChildren<TextMeshProUGUI>();
        if (texto != null)
            texto.color = activo ? colorActivo : colorInactivo;

        var icono = tab.transform.Find("Icono")?.GetComponent<Image>();
        if (icono != null)
            icono.color = activo ? colorActivo : colorInactivo;

        var underline = tab.transform.Find("Underline")?.GetComponent<Image>();
        if (underline != null)
            underline.color = activo ? colorActivo : Color.clear;
    }

    void GenerarBotones()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        for (int i = 0; i < filtros.Count; i++)
        {
            if (filtros[i].categoria != categoriaActiva) continue;

            int index = i;
            GameObject btn = Instantiate(botonPrefab, content);

            var image = btn.GetComponent<Image>();
            if (image != null && filtros[i].preview != null)
                image.sprite = filtros[i].preview;

            var texto = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (texto != null)
                texto.text = filtros[i].nombre;

            // Muestra visualmente si el filtro ya está activo
            ActualizarVisualBoton(btn, filtrosActivos.Contains(index));

            btn.GetComponent<Button>().onClick.AddListener(() => ToggleFiltro(index, btn));
        }
    }

    public void ToggleFiltro(int index, GameObject btn)
    {
        if (filtrosActivos.Contains(index))
        {
            filtrosActivos.Remove(index);
            DesactivarFiltro(index);
            ActualizarVisualBoton(btn, false);
            ColorPaleta.Instance?.OcultarPaleta();
        }
        else
        {
            filtrosActivos.Add(index);
            ActivarFiltro(index);
            ActualizarVisualBoton(btn, true);

            // Muestra paleta solo si es accesorio 3D con colores
            var filtro = filtros[index];
            if (filtro.materialMaquillaje == null &&
                filtro.coloresDisponibles != null &&
                filtro.coloresDisponibles.Length > 0)
            {
                ColorPaleta.Instance?.MostrarPaleta(
                    filtro.nombreModelo,
                    filtro.coloresDisponibles
                );
            }
        }
    }

    void ActivarFiltro(int index)
    {
        var filtro = filtros[index];
        if (filtro.materialMaquillaje != null)
            AplicarMaquillaje(filtro.materialMaquillaje);
        else
            SetModeloEnCaras(filtro.nombreModelo, true);
    }

    void DesactivarFiltro(int index)
    {
        var filtro = filtros[index];
        if (filtro.materialMaquillaje != null)
            QuitarMaquillaje(filtro.materialMaquillaje);
        else
            SetModeloEnCaras(filtro.nombreModelo, false);
    }

    // Visual del botón activo vs inactivo
    void ActualizarVisualBoton(GameObject btn, bool activo)
    {
        // Asegura que el TabBar no se vea afectado
        tabMaquillaje.transform.parent
                     .GetComponent<RectTransform>()
                     .localScale = Vector3.one;

        btn.GetComponent<RectTransform>()
           .DOScale(activo ? new Vector3(1.15f, 1.15f, 1.15f) : Vector3.one, 0.2f);

        var image = btn.GetComponent<Image>();
        if (image != null)
            image.color = activo ? new Color(0.788f, 0.659f, 0.298f, 1f) : Color.white;
    }

    private void SetModeloEnCaras(string nombreModelo, bool activo)
    {
        foreach (var face in faceManager.trackables)
        {
            var modelo = face.transform.Find(nombreModelo);
            if (modelo != null)
                modelo.gameObject.SetActive(activo);
            else
                Debug.LogWarning($"No se encontró '{nombreModelo}' en {face.trackableId}");
        }
    }

    private void AplicarMaquillaje(Material mat)
    {
        foreach (var face in faceManager.trackables)
        {
            var renderer = face.GetComponent<MeshRenderer>();
            if (renderer == null) continue;

            // Evita duplicados
            foreach (var m in renderer.materials)
                if (m.name == mat.name + " (Instance)") return;

            var mats = renderer.materials;
            var newMats = new Material[mats.Length + 1];
            newMats[0] = mat;
            mats.CopyTo(newMats, 1);
            renderer.materials = newMats;
        }
    }

    private void QuitarMaquillaje(Material mat)
    {
        foreach (var face in faceManager.trackables)
        {
            var renderer = face.GetComponent<MeshRenderer>();
            if (renderer == null) continue;

            var mats = renderer.materials;
            var newMats = new List<Material>();

            foreach (var m in mats)
            {
                // Quita solo el material específico
                if (m.name != mat.name + " (Instance)")
                    newMats.Add(m);
            }

            renderer.materials = newMats.ToArray();
        }
    }
}