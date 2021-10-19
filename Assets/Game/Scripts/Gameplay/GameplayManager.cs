using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : SingletonMonobehaviour<GameplayManager>
{
    [SerializeField] GameObject _pacmanPrefab = null;
    
    void Start() {
        SetupCharacters();
    }
    
    void SetupCharacters() {
        Vector3 position = GridBoard.Instance.GetSpawnWorldPosition(Character.Pacman);
        Instantiate(_pacmanPrefab, position, Quaternion.identity);
    }
}
