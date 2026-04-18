using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{

    //script que maneja los eventos de los menus
    public static GameManager Instance { get; private set; }

    public event Action OnMainMenu;
    public event Action OnItemsMenu;
    public event Action OnARPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GoToMainMenu() => OnMainMenu?.Invoke();
    public void GoToItemsMenu() => OnItemsMenu?.Invoke();
    public void GoToARPosition() => OnARPosition?.Invoke();
}