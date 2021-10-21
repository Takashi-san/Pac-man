using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Ghost Behaviour/Inky")]
public class InkyBehaviour : GhostBehaviour
{
    public override Vector3Int GetChasePosition(GameObject p_owner) {
        Vector3Int pacmanPosition = GridBoard.Instance.GetPositionWorldToCell(GameplayManager.Instance.Pacman.transform.position);
        Vector2Int pacmanDirection = GameplayManager.Instance.Pacman.MoveDirection;
        Vector3Int desiredPosition = pacmanPosition + 2 * (Vector3Int)pacmanDirection;
        
        Vector3Int blinkyPosition = GameplayManager.Instance.GetGhostPosition(Character.Blinky);
        Vector3Int diff = blinkyPosition - desiredPosition;
        desiredPosition = desiredPosition - diff;
        return desiredPosition;
    }
}
