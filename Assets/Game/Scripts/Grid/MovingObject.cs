using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public event System.Action OnMovedToCell;
    public bool IsMoving => _moveCoroutine != null;
    
    [SerializeField] protected float _speed = 1;
    Vector2Int _moveDirection = Vector2Int.up;
    Vector2Int _desiredDirection = Vector2Int.up;
    Coroutine _moveCoroutine = null;

    void Awake() {
    }
    
    void Start() {
        var cellPosition = GridBoard.Instance.GetPositionWorldToCell(transform.position);
        transform.position = GridBoard.Instance.GetPositionCellToWorld(cellPosition);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            ChangeDirection(Vector2Int.up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            ChangeDirection(Vector2Int.down);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            ChangeDirection(Vector2Int.left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            ChangeDirection(Vector2Int.right);
        }
    }

    protected void ChangeDirection(Vector2Int p_direction) {
        _desiredDirection = p_direction;
        if (CanChangeDirection()) {
            _moveDirection = _desiredDirection;
            StartMoveToCell(GetCellOnDirection((Vector3Int)_moveDirection));
        }
    }

    bool CanChangeDirection() {
        if (!IsOnCellPosition()) {
            if (!IsSameWayToMovement(_desiredDirection)) {
                return false;
            }
        }
        return GridBoard.Instance.IsTileWalkable(GetCellOnDirection((Vector3Int)_desiredDirection));
    }

    bool IsOnCellPosition() {
        return (Vector2)transform.position == (Vector2)GridBoard.Instance.GetPositionWorldToWorld(transform.position);
    }

    bool IsSameWayToMovement(Vector2Int p_direction) {
        return p_direction == _moveDirection || p_direction == -_moveDirection;
    }

    Vector3Int GetCellOnDirection(Vector3Int p_direction) {
        Vector3Int currentCell = GridBoard.Instance.GetPositionWorldToCell(transform.position);
        Vector3Int directionCell = currentCell + p_direction;
        
        if (IsOnCellPosition()) {
            return directionCell;
        }
        if (!IsSameWayToMovement((Vector2Int)p_direction)) {
            return directionCell;
        }
        
        // Vector3 currentPosition = GridBoard.Instance.GetPositionCellToWorld(currentCell);
        Vector3 directionPosition = GridBoard.Instance.GetPositionCellToWorld(directionCell);
        // Vector3 diffCurrent = transform.position - currentPosition;
        Vector3 diffDirection = transform.position - directionPosition;
        if (diffDirection.magnitude > 1) { // Considerando que todos os tiles possuem tamanho de 1 x 1.
            return currentCell;
        }
        else {
            return directionCell;
        }
    }

    void StartMoveToCell(Vector3Int p_cellPosition) {
        if (_moveCoroutine != null) {
            StopCoroutine(_moveCoroutine);
        }
        _moveCoroutine = StartCoroutine(MoveToCell(p_cellPosition));
    }

    IEnumerator MoveToCell(Vector3Int p_cellPosition) {
        Vector3 targetPosition = GridBoard.Instance.GetPositionCellToWorld(p_cellPosition);

        while(transform.position != targetPosition) {
            yield return null;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);
        }

        _moveCoroutine = null;
        OnMovedToCell?.Invoke();
    }
}
