using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIEndGame : MonoBehaviour
{
    [SerializeField] GameObject _view = null;
    [SerializeField] Text _text = null;

    void Start() {
        GameplayManager.Instance.OnGameEnded += OnGameEnded;
        _view.SetActive(false);
    }

    void OnGameEnded(bool p_won) {
        if (p_won) {
            _text.text = "YOU WIN!";
        }
        else {
            _text.text = "YOU LOSE!";
        }

        _view.SetActive(true);
    }

    public void RestartButton() {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }
}
