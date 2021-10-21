using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : SingletonMonobehaviour<GameplayManager>
{
    public event System.Action<float> OnGotBigPellet;
    public event System.Action<bool> OnGameEnded;
    public event System.Action OnGameStarted;
    public event System.Action<Ghost.State> OnChangeGhostDesiredState;
    public Ghost.State DesiredGhostState => _desiredGhostState;
    
    [HideInInspector] public PacMan Pacman = null;

    [Header("Prefabs")]
    [SerializeField] GameObject _pacmanPrefab = null;
    [SerializeField] GameObject _blinkyPrefab = null;
    [SerializeField] GameObject _pinkyPrefab = null;
    [SerializeField] GameObject _inkyPrefab = null;
    [SerializeField] GameObject _clydePrefab = null;

    [Header("Gameplay")]
    [SerializeField] float _frightenedDuration = 1;
    [SerializeField] float _changeGhostStateTime = 10;
    [SerializeField] [Min(1)] float _timeMultiplier = 1;

    List<Ghost> _ghosts = new List<Ghost>();
    List<GameObject> _pellets = new List<GameObject>();
    Coroutine _gameStartRoutine = null;
    Coroutine _ghostStateRoutine = null;
    Ghost.State _desiredGhostState = Ghost.State.Chase;
    
    void Start() {
        SetupCharacters();
    }
    
    void SetupCharacters() {
        Vector3 position = GridBoard.Instance.GetSpawnWorldPosition(Character.Pacman);
        Pacman = Instantiate(_pacmanPrefab, position, Quaternion.identity).GetComponent<PacMan>();
        Pacman.OnDie += LoseGame;

        position = GridBoard.Instance.GetSpawnWorldPosition(Character.Blinky);
        Ghost ghost = Instantiate(_blinkyPrefab, position, Quaternion.identity).GetComponent<Ghost>();
        ghost.ChangeState(Ghost.State.Waiting);
        _ghosts.Add(ghost);

        position = GridBoard.Instance.GetSpawnWorldPosition(Character.Pinky);
        ghost = Instantiate(_pinkyPrefab, position, Quaternion.identity).GetComponent<Ghost>();
        ghost.ChangeState(Ghost.State.Waiting);
        _ghosts.Add(ghost);

        position = GridBoard.Instance.GetSpawnWorldPosition(Character.Inky);
        ghost = Instantiate(_inkyPrefab, position, Quaternion.identity).GetComponent<Ghost>();
        ghost.ChangeState(Ghost.State.Waiting);
        _ghosts.Add(ghost);

        position = GridBoard.Instance.GetSpawnWorldPosition(Character.Clyde);
        ghost = Instantiate(_clydePrefab, position, Quaternion.identity).GetComponent<Ghost>();
        ghost.ChangeState(Ghost.State.Waiting);
        _ghosts.Add(ghost);
    }

    public void StartGame() {
        _gameStartRoutine = StartCoroutine(StartGameSequence());
        _ghostStateRoutine = StartCoroutine(ChangeGhostStateRoutine());
    }

    public Vector3Int GetGhostPosition(Character p_character) {
        Ghost ghost = _ghosts.Find((Ghost ghost) => { return ghost.Character == p_character; });
        if (ghost != null) {
            return GridBoard.Instance.GetPositionWorldToCell(ghost.transform.position);
        }
        return Vector3Int.zero;
    }

    IEnumerator StartGameSequence() {
        OnGameStarted?.Invoke();

        List<Ghost> ghosts = _ghosts.FindAll(
            delegate(Ghost ghost) {
                return ghost.Character == Character.Blinky;
            }
        );
        foreach (var ghost in ghosts) {
            ghost.ChangeState(Ghost.State.MoveOutRespawn);
        }
        yield return new WaitForSeconds(2);

        ghosts = _ghosts.FindAll(
            delegate(Ghost ghost) {
                return ghost.Character == Character.Pinky;
            }
        );
        foreach (var ghost in ghosts) {
            ghost.ChangeState(Ghost.State.MoveOutRespawn);
        }
        yield return new WaitForSeconds(2);

        ghosts = _ghosts.FindAll(
            delegate(Ghost ghost) {
                return ghost.Character == Character.Inky;
            }
        );
        foreach (var ghost in ghosts) {
            ghost.ChangeState(Ghost.State.MoveOutRespawn);
        }
        yield return new WaitForSeconds(2);

        ghosts = _ghosts.FindAll(
            delegate(Ghost ghost) {
                return ghost.Character == Character.Clyde;
            }
        );
        foreach (var ghost in ghosts) {
            ghost.ChangeState(Ghost.State.MoveOutRespawn);
        }

        _gameStartRoutine = null;
    }

    IEnumerator ChangeGhostStateRoutine() {
        int count = 1;

        _desiredGhostState = Ghost.State.Scatter;
        while (true) {
            float time = _changeGhostStateTime;
            if (_desiredGhostState == Ghost.State.Chase) {
                _desiredGhostState = Ghost.State.Scatter;
                time = time / Mathf.Pow(_timeMultiplier, count);
            }
            else {
                _desiredGhostState = Ghost.State.Chase;
                time = time * Mathf.Pow(_timeMultiplier, count);
            }
            count++;
            OnChangeGhostDesiredState?.Invoke(_desiredGhostState);
            yield return new WaitForSeconds(time); 
        }
    }

    public void GotBigPellet() {
        OnGotBigPellet?.Invoke(_frightenedDuration);
    }

    public void AddPellet(GameObject p_pellet) {
        _pellets.Add(p_pellet);
    }

    public void RemovePellet(GameObject p_pellet) {
        _pellets.Remove(p_pellet);
        if (_pellets.Count == 0) {
            WinGame();
        }
    }

    public void AddScore(int p_score) {
        // wip
    }

    void GameEnded() {
        if (_gameStartRoutine != null) {
            StopCoroutine(_gameStartRoutine);
        }
        if (_ghostStateRoutine != null) {
            StopCoroutine(_ghostStateRoutine);
        }
    }

    void LoseGame() {
        print("Lose");
        GameEnded();
        OnGameEnded?.Invoke(false);
    }

    void WinGame() {
        print("Win");
        GameEnded();
        OnGameEnded?.Invoke(true);
    }
}
