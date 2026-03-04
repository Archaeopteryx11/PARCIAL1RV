using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UIElements;

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

        StartCoroutine(SetSpawnNextFrame(player, index));
        Debug.Log($"[PlayerSpawner] Jugador {index + 1} unido. Dispositivo: {player.devices[0].displayName}");
    }

    private IEnumerator SetSpawnNextFrame(PlayerInput player, int index)
    {
        yield return null; // esperar un frame

        if (player == null) yield break;
        if (spawnPoints[index] == null)
        {
            Debug.LogWarning($"[PlayerSpawner] spawnPoint{index} no está asignado en el Inspector.");
            yield break;
        }

        // Si tiene CharacterController hay que desactivarlo antes de mover
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.SetPositionAndRotation(
            spawnPoints[index].position,
            spawnPoints[index].rotation
        );

        if (cc != null) cc.enabled = true;
    }

    public void RespawnAll()
    {
        foreach (var player in PlayerInput.all)
        {
            int index = player.playerIndex;
            if (index < spawnPoints.Length && spawnPoints[index] != null)
            {
                StartCoroutine(SetSpawnNextFrame(player, index));
            }
        }
    }
}
