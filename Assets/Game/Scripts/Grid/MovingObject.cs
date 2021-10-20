using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public event System.Action OnMovedToCell;
    public bool IsMoving => _moveCoroutine != null;
    public Vector2Int MoveDirection => (Vector2Int)_moveDirection;
    
    [SerializeField] protected float _speed = 1;
    protected List<TerrainType> _walkableOn = new List<TerrainType>{TerrainType.Walkable, TerrainType.Teleport};
    Vector3Int _moveDirection = Vector3Int.up;
    Vector3Int _desiredDirection = Vector3Int.up;
    Coroutine _moveCoroutine = null;
    
    void Start() {
        var cellPosition = GridBoard.Instance.GetPositionWorldToCell(transform.position);
        transform.position = GridBoard.Instance.GetPositionCellToWorld(cellPosition);
    }

    #region Protected Methods
    protected void ChangeDirection(Vector2Int p_direction) {
        _desiredDirection = (Vector3Int)p_direction;
        if (CanChangeDirection()) {
            _moveDirection = _desiredDirection;
            StartMoveToCell(GetCellOnDirection(_moveDirection));
        }
    }

    protected void KeepMoving() {
        if (CanChangeDirection()) {
            _moveDirection = _desiredDirection;
            StartMoveToCell(GetCellOnDirection(_moveDirection));
        }
        else if (IsWalkableOnDirection(_moveDirection))  {
            StartMoveToCell(GetCellOnDirection(_moveDirection));
        }
    }

    protected void StopMoving() {
        if (_moveCoroutine != null) {
            StopCoroutine(_moveCoroutine);
        }
    }
    #endregion

    #region Bool checks
    bool CanChangeDirection() {
        if (!IsOnCellPosition()) {
            if (!IsSameWayToMovement(_desiredDirection)) {
                return false;
            }
        }
        return IsWalkableOnDirection(_desiredDirection);
    }

    bool IsOnCellPosition() {
        return (Vector2)transform.position == (Vector2)GridBoard.Instance.GetPositionWorldToWorld(transform.position);
    }

    bool IsSameWayToMovement(Vector3Int p_direction) {
        return p_direction == _moveDirection || p_direction == -_moveDirection;
    }

    protected bool IsWalkableOnDirection(Vector3Int p_direction) {
        return GridBoard.Instance.IsTileWalkable(GetCellOnDirection(p_direction), _walkableOn);
    }
    #endregion

    Vector3Int GetCellOnDirection(Vector3Int p_direction) {
        Vector3Int currentCell = GridBoard.Instance.GetPositionWorldToCell(transform.position);
        Vector3Int directionCell = currentCell + p_direction;
        
        if (IsOnCellPosition()) {
            return directionCell;
        }
        if (!IsSameWayToMovement(p_direction)) {
            return directionCell;
        }
        
        Vector3 directionPosition = GridBoard.Instance.GetPositionCellToWorld(directionCell);
        Vector3 diffDirection = transform.position - directionPosition;
        if (diffDirection.magnitude > 1) { // Considerando que todos os tiles possuem tamanho de 1x1.
            return currentCell;
        }
        else {
            return directionCell;
        }
    }

    #region Movement
    void StartMoveToCell(Vector3Int p_cellPosition) {
        StopMoving();
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
    #endregion
}
