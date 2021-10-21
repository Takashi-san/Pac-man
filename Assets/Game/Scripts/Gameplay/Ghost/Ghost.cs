using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MovingObject
{
    public enum State {
        Chase,
        Scatter,
        Eaten,
        Frightened,
        Waiting,
        MoveOutRespawn
    }

    public event System.Action OnGotEaten;
    public Character Character => _behaviour.Ghost;

    [SerializeField] GhostBehaviour _behaviour = null;
    [SerializeField] SpriteRenderer _visual = null;
    [SerializeField] Color _originalColor = Color.white;
    State _state = State.Scatter;
    List<State> _backWalkable = new List<State>{State.Waiting, State.MoveOutRespawn};
    List<State> _frightenedAllowed = new List<State>{State.Chase, State.Scatter, State.Frightened};
    List<State> _gameplayDesireAllowed = new List<State>{State.Chase, State.Scatter};
    Coroutine _frightenedCoroutine = null;
    
    void Start() {
        OnMovedToCell += AvaliateNextStep;
        _speed = _behaviour.Speed;

        GameplayManager.Instance.OnGotBigPellet += BecomeFrightened;
        GameplayManager.Instance.OnGameEnded += OnGameEnded;
        GameplayManager.Instance.OnChangeGhostDesiredState += OnChangeGhostDesiredState;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            ChangeState(State.Chase);
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            ChangeState(State.Scatter);
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            ChangeState(State.Eaten);
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            ChangeState(State.Frightened);
        }
        if (Input.GetKeyDown(KeyCode.T)) {
            ChangeState(State.Waiting);
        }
        if (Input.GetKeyDown(KeyCode.Y)) {
            ChangeState(State.MoveOutRespawn);
        }
    }

    public void ChangeState(State p_state) {
        if (_state == p_state) {
            return;
        }
        // print($"[ghost] change state to: {p_state}");
        if (_state == State.Frightened) {
            StopFrightenedCoroutine();
        }
        _state = p_state;
        SetupWalkableOn();

        switch(_state) {
            case State.Frightened:
                _visual.color = Color.black;
                break;
            
            case State.Eaten:
                _visual.color = Color.white;
                break;
            
            default:
                _visual.color = _originalColor;
                break;
        }
        
        switch(_state) {
            case State.Chase:
            case State.Scatter:
            case State.Frightened:
                if (IsWalkableOnDirection(-(Vector3Int)MoveDirection)) {
                    ChangeDirection(-MoveDirection);
                    if (!IsMoving) {
                        KeepMoving();
                    }
                }
                else {
                    AvaliateNextStep();
                }
                break;
            
            default:
                AvaliateNextStep();
                break;
        }
    }

    void SetupWalkableOn() {
        switch (_state) {
            case State.MoveOutRespawn:
            case State.Eaten:
                _walkableOn = new List<TerrainType>{TerrainType.Walkable, TerrainType.Teleport, TerrainType.RespawnWall};
                break;

            default:
                _walkableOn = new List<TerrainType>{TerrainType.Walkable, TerrainType.Teleport};
                break;
        }
    }

    void StopFrightenedCoroutine() {
        if (_frightenedCoroutine != null) {
            StopCoroutine(_frightenedCoroutine);
        }
    }
    
    void BecomeFrightened(float p_duration) {
        if (!_frightenedAllowed.Contains(_state)) {
            return;
        }
        
        ChangeState(State.Frightened);
        StopFrightenedCoroutine();
        _frightenedCoroutine = StartCoroutine(WaitFrightenedDuration(p_duration));
    }

    void OnGameEnded(bool p_won) {
        StopFrightenedCoroutine();
        StopMoving();
    }

    void OnChangeGhostDesiredState(State p_state) {
        if (!_gameplayDesireAllowed.Contains(_state)) {
            return;
        }
        
        ChangeState(p_state);
    }

    void AvaliateNextStep() {
        Vector3Int desiredCell = Vector3Int.zero;
        Vector3Int position = GridBoard.Instance.GetPositionWorldToCell(transform.position);

        switch (_state) {
            case State.Chase:
                desiredCell = _behaviour.GetChasePosition(gameObject);
                break;
            
            case State.Scatter:
                desiredCell = GridBoard.Instance.GetScatterCellPosition(_behaviour.Ghost);
                break;
            
            case State.Eaten:
                desiredCell = GridBoard.Instance.GetRespawnCellPosition();
                if (position == desiredCell + Vector3Int.down) {
                    desiredCell = desiredCell + Vector3Int.down;
                }
                if (position == desiredCell + 2 * Vector3Int.down) {
                    ChangeState(State.Waiting);
                    StartCoroutine(WaitRespawn());
                    return;
                }
                break;
            
            case State.Frightened:
                desiredCell = GetFrightenedPosition();
                break;
            
            case State.Waiting:
                // desiredCell = GetWaitingPosition();
                StopMoving();
                return;
            
            case State.MoveOutRespawn:
                desiredCell = GridBoard.Instance.GetRespawnCellPosition();
                if (position == desiredCell) {
                    ChangeState(GameplayManager.Instance.DesiredGhostState);
                }
                break;
        }

        Vector2Int desiredDirection = GetClosestDirectionToCell(desiredCell);
        if (desiredDirection == Vector2Int.zero) {
            KeepMoving();
        }
        else {
            ChangeDirection(desiredDirection);
            if (!IsMoving) {
                KeepMoving();
            }
        }
    }

    Vector2Int GetClosestDirectionToCell(Vector3Int p_position) {
        Vector2Int result = Vector2Int.zero;
        Vector3Int currentPosition = GridBoard.Instance.GetPositionWorldToCell(transform.position);
        Dictionary<Vector2Int, float> distances = new Dictionary<Vector2Int, float>();

        if (GridBoard.Instance.IsTileWalkable(currentPosition + Vector3Int.up, _walkableOn)) {
            distances.Add(Vector2Int.up, Vector3Int.Distance(p_position, currentPosition + Vector3Int.up));
        }
        if (GridBoard.Instance.IsTileWalkable(currentPosition + Vector3Int.down, _walkableOn)) {
            distances.Add(Vector2Int.down, Vector3Int.Distance(p_position, currentPosition + Vector3Int.down));
        }
        if (GridBoard.Instance.IsTileWalkable(currentPosition + Vector3Int.left, _walkableOn)) {
            distances.Add(Vector2Int.left, Vector3Int.Distance(p_position, currentPosition + Vector3Int.left));
        }
        if (GridBoard.Instance.IsTileWalkable(currentPosition + Vector3Int.right, _walkableOn)) {
            distances.Add(Vector2Int.right, Vector3Int.Distance(p_position, currentPosition + Vector3Int.right));
        }
        
        if (!_backWalkable.Contains(_state)) {
            distances.Remove(-MoveDirection);
        }

        float min = float.MaxValue;
        foreach (var kvp in distances) {
            if (kvp.Value < min) {
                min = kvp.Value;
                result = kvp.Key;
            }
        }
        
        return result;
    }

    Vector3Int GetFrightenedPosition() {
        Vector3Int currentPosition = GridBoard.Instance.GetPositionWorldToCell(transform.position);
        Vector3Int result = currentPosition;
        List<Vector2Int> directions = new List<Vector2Int>();

        if (GridBoard.Instance.IsTileWalkable(currentPosition + Vector3Int.up, _walkableOn)) {
            directions.Add(Vector2Int.up);
        }
        if (GridBoard.Instance.IsTileWalkable(currentPosition + Vector3Int.down, _walkableOn)) {
            directions.Add(Vector2Int.down);
        }
        if (GridBoard.Instance.IsTileWalkable(currentPosition + Vector3Int.left, _walkableOn)) {
            directions.Add(Vector2Int.left);
        }
        if (GridBoard.Instance.IsTileWalkable(currentPosition + Vector3Int.right, _walkableOn)) {
            directions.Add(Vector2Int.right);
        }
        directions.Remove(-MoveDirection);

        if (directions.Count != 0) {
            int random = Random.Range(0, directions.Count);
            result = currentPosition + (Vector3Int)directions[random];
        }
        
        return result;
    }

    Vector3Int GetWaitingPosition() {
        Vector3Int currentPosition = GridBoard.Instance.GetPositionWorldToCell(transform.position);
        if (GridBoard.Instance.IsTileWalkable(currentPosition + Vector3Int.up, _walkableOn)) {
            return currentPosition + Vector3Int.up;
        }
        else {
            return currentPosition + Vector3Int.down;
        }
    }

    IEnumerator WaitRespawn() {
        yield return new WaitForSeconds(_behaviour.RespawnTime);
        ChangeState(State.MoveOutRespawn);
    }

    IEnumerator WaitFrightenedDuration(float p_duration) {
        yield return new WaitForSeconds(p_duration);
        ChangeState(GameplayManager.Instance.DesiredGhostState);
        _frightenedCoroutine = null;
    }

    void GotEaten() {
        StopFrightenedCoroutine();
        ChangeState(State.Eaten);
        OnGotEaten?.Invoke();
    }
    
    private void OnTriggerEnter2D(Collider2D p_other) {
        PacMan pacman = p_other.GetComponent<PacMan>();
        if (pacman != null) {
            if (_state == State.Frightened) {
                GotEaten();
            }
            else if (_state != State.Eaten) {
                pacman.Die();
            }
        }
    }
}
