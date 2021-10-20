using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Ghost Behaviour/Blinky")]
public class BlinkyBehaviour : GhostBehaviour
{
    public override Vector3Int GetChasePosition() {
        return GridBoard.Instance.GetPositionWorldToCell(GameplayManager.Instance.Pacman.transform.position);
    }
}
