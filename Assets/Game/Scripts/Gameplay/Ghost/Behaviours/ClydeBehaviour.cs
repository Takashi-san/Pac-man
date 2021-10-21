using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Ghost Behaviour/Clyde")]
public class ClydeBehaviour : GhostBehaviour
{
    public override Vector3Int GetChasePosition(GameObject p_owner) {
        Vector3Int pacmanPosition = GridBoard.Instance.GetPositionWorldToCell(GameplayManager.Instance.Pacman.transform.position);
        Vector3Int currentPosition = GridBoard.Instance.GetPositionWorldToCell(p_owner.transform.position);
        Vector3Int diff = pacmanPosition - currentPosition;
        int diffMag2 = diff.x * diff.x + diff.y * diff.y;

        Vector3Int desiredPosition = pacmanPosition;
        if (diffMag2 < 64) {
            desiredPosition = GridBoard.Instance.GetScatterCellPosition(Character.Blinky);
        }
        
        return desiredPosition;
    }
}
