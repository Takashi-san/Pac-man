using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan : MovingObject
{
    public event System.Action OnDie;

    bool _canControl = false;
    bool _isDead = false;
    
    public void Die() {
        _isDead = true;
        OnDie?.Invoke();
        StopMoving();
    }
    
    void Start() {
        OnMovedToCell += KeepMoving;
        GameplayManager.Instance.OnGameEnded += OnGameEnded;
        GameplayManager.Instance.OnGameStarted += () => { _canControl = true; };
    }

    void OnGameEnded(bool p_won) {
        _canControl = false;
        StopMoving();
    }
    
    void Update() {
        if (!_isDead && _canControl) {
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
}
