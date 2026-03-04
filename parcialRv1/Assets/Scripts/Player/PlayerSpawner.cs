using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

/// <summary>
/// Gestiona el spawn de los 4 jugadores y asigna automáticamente
/// el Viewport Rect correcto a cada cámara según su índice.
///
/// SETUP en Unity:
/// 1. Crea un GameObject vacío llamado "PlayerSpawner" en la escena
/// 2. Adjunta este script
/// 3. Asigna el prefab del jugador al campo "playerPrefab"
/// 4. Crea 4 GameObjects vacíos como SpawnPoints y asígnalos
/// 5. El PlayerInputManager se agrega automáticamente
/// </summary>
[RequireComponent(typeof(PlayerInputManager))]
public class PlayerSpawner : MonoBehaviour
{
    [Header("Prefab del Jugador")]
    [Tooltip("Prefab con: PlayerInput, FPSController, FPSCameraController")]
    public GameObject playerPrefab;

    [Header("Puntos de Spawn (uno por jugador)")]
    public Transform spawnPoint0;
    public Transform spawnPoint1;
    public Transform spawnPoint2;
    public Transform spawnPoint3;

    // ── Viewports para 4 jugadores (split screen en cuadrícula 2x2) ──
    //  P1 arriba-izquierda  |  P2 arriba-derecha
    //  P3 abajo-izquierda   |  P4 abajo-derecha
    private static readonly Rect[] viewports = new Rect[]
    {
        new Rect(0f,    0.5f,  0.5f, 0.5f),   // Jugador 1 — arriba izquierda
        new Rect(0.5f,  0.5f,  0.5f, 0.5f),   // Jugador 2 — arriba derecha
        new Rect(0f,    0f,    0.5f, 0.5f),   // Jugador 3 — abajo izquierda
        new Rect(0.5f,  0f,    0.5f, 0.5f),   // Jugador 4 — abajo derecha
    };

    private PlayerInputManager inputManager;
    private Transform[] spawnPoints;
    private int playerCount = 0;

    void Awake()
    {
        inputManager = GetComponent<PlayerInputManager>();

        // Configurar el PlayerInputManager por código
        inputManager.joinBehavior        = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
        inputManager.playerPrefab        = playerPrefab;
        inputManager.notificationBehavior = PlayerNotifications.InvokeUnityEvents;

        spawnPoints = new Transform[]
        {
            spawnPoint0, spawnPoint1, spawnPoint2, spawnPoint3
        };
    }

    void OnEnable()
    {
        inputManager.onPlayerJoined += OnPlayerJoined;
    }

    void OnDisable()
    {
        inputManager.onPlayerJoined -= OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerInput player)
    {
        int index = player.playerIndex;
        if (index >= 4) return;

        // 1. Posicionar en spawn point
        if (spawnPoints[index] != null)
        {
            player.transform.position = spawnPoints[index].position;
            player.transform.rotation = spawnPoints[index].rotation;
        }

        // 2. Asignar viewport a la cámara del jugador
        Camera cam = player.GetComponentInChildren<Camera>();
        if (cam != null)
        {
            cam.rect = viewports[index];
            cam.depth = index; // Evitar conflictos de render order
        }

        // 3. Informar al FPSController el índice del jugador
        MovePlayer fps = player.GetComponent<MovePlayer>();
        if (fps != null) fps.SetPlayerIndex(index);

        playerCount++;
        Debug.Log($"[PlayerSpawner] Jugador {index + 1} unido. Dispositivo: {player.devices[0].displayName}");
    }

    public void RespawnAll()
    {
        foreach (var player in PlayerInput.all)
        {
            int index = player.playerIndex;
            if (index < spawnPoints.Length && spawnPoints[index] != null)
            {
                player.transform.position = spawnPoints[index].position;
                player.transform.rotation = spawnPoints[index].rotation;
            }
        }
    }
}
