using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Ghost Behaviour/Pinky")]
public class PinkyBehaviour : GhostBehaviour
{
    public override Vector3Int GetChasePosition(GameObject p_owner) {
        Vector3Int pacmanPosition = GridBoard.Instance.GetPositionWorldToCell(GameplayManager.Instance.Pacman.transform.position);
        Vector2Int pacmanDirection = GameplayManager.Instance.Pacman.MoveDirection;
        Vector3Int desiredPosition = pacmanPosition + 4 * (Vector3Int)pacmanDirection;
        return desiredPosition;
    }
}
