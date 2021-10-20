using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MovingObject
{
    void Start() {
        OnMovedToCell += KeepMoving;
    }
    
    void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            ChangeDirection(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            ChangeDirection(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            ChangeDirection(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            ChangeDirection(Vector2Int.right);
        }
    }
}
