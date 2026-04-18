using UnityEngine;
using DG.Tweening;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;       // MainMenu
    [SerializeField] private RectTransform carruselPanel; // CarruselPanel

    void Start()
    {
        GameManager.Instance.OnMainMenu += MostrarMainMenu;
        GameManager.Instance.OnARPosition += MostrarARPosition;

        // Estado inicial
        carruselPanel.localScale = Vector3.zero;
    }

    private void MostrarMainMenu()
    {
        // Mostrar MainMenu
        mainMenu.transform.GetChild(0).DOScale(Vector3.one, 0.3f); // ShowItems
        mainMenu.transform.GetChild(1).DOScale(Vector3.one, 0.3f); // ScreenShot

        // Ocultar carrusel
        carruselPanel.DOScale(Vector3.zero, 0.3f);
    }

    private void MostrarARPosition()
    {
        // Ocultar MainMenu
        mainMenu.transform.GetChild(0).DOScale(Vector3.zero, 0.3f); // ShowItems
        mainMenu.transform.GetChild(1).DOScale(Vector3.zero, 0.3f); // ScreenShot

        // Mostrar carrusel con bounce
        carruselPanel.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
    }

    void OnDestroy()
    {
        GameManager.Instance.OnMainMenu -= MostrarMainMenu;
        GameManager.Instance.OnARPosition -= MostrarARPosition;
    }
}