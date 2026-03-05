using UnityEngine;

/// <summary>
/// Componente que va en cada punto de reparación cooperativa.
/// Agrega este script a los GameObjects "PuntoA" y "PuntoB".
/// No necesita configuración manual — CoopRepairMission lo inicializa solo.
/// </summary>
public class CoopPoint : MonoBehaviour, IInteractable
{
    public bool       IsOccupied    { get; private set; } = false;
    public MovePlayer CurrentPlayer { get; private set; } = null;

    private CoopRepairMission parentMission;

    public void Setup(CoopRepairMission mission)
    {
        parentMission = mission;
    }

    public void Interact(MovePlayer player)
    {
        if (parentMission == null || parentMission.IsCompleted) return;

        if (!IsOccupied)
        {
            IsOccupied    = true;
            CurrentPlayer = player;
            Debug.Log($"[CoopPoint] {player.name} ocupó {gameObject.name}");
        }
        else if (CurrentPlayer == player)
        {
            IsOccupied    = false;
            CurrentPlayer = null;
            Debug.Log($"[CoopPoint] {player.name} liberó {gameObject.name}");
        }
    }
}
