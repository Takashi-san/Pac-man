using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStartGame : MonoBehaviour
{
    [SerializeField] GameObject _view = null;
    
    public void StartGameButton() {
        _view.SetActive(false);
        GameplayManager.Instance.StartGame();
    }
}
