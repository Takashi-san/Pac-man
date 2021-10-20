using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : SingletonMonobehaviour<GameplayManager>
{
    [HideInInspector] public PacMan Pacman = null;

    [SerializeField] GameObject _pacmanPrefab = null;
    [SerializeField] GameObject _blinkyPrefab = null;
    
    void Start() {
        SetupCharacters();
    }
    
    void SetupCharacters() {
        Vector3 position = GridBoard.Instance.GetSpawnWorldPosition(Character.Pacman);
        Pacman = Instantiate(_pacmanPrefab, position, Quaternion.identity).GetComponent<PacMan>();

        position = GridBoard.Instance.GetSpawnWorldPosition(Character.Blinky);
        Instantiate(_blinkyPrefab, position, Quaternion.identity);
    }
}
