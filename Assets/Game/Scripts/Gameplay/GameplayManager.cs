using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : SingletonMonobehaviour<GameplayManager>
{
    public event System.Action<float> OnGotBigPellet;
    public event System.Action<bool> OnGameEnded;
    public event System.Action OnGameStarted;
    
    [HideInInspector] public PacMan Pacman = null;

    [Header("Prefabs")]
    [SerializeField] GameObject _pacmanPrefab = null;
    [SerializeField] GameObject _blinkyPrefab = null;

    [Header("Gameplay")]
    [SerializeField] float _frightenedDuration = 1;

    List<Ghost> _ghosts = new List<Ghost>();
    List<GameObject> _pellets = new List<GameObject>();
    
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
    }

    public void StartGame() {
        foreach (var ghost in _ghosts) {
            ghost.ChangeState(Ghost.State.MoveOutRespawn);
        }

        OnGameStarted?.Invoke();
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

    void LoseGame() {
        print("Lose");
        OnGameEnded?.Invoke(false);
    }

    void WinGame() {
        print("Win");
        OnGameEnded?.Invoke(true);
    }
}
