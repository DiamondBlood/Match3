using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestartGame : MonoBehaviour
{
    [SerializeField] private Board _board;
    [SerializeField] private GameObject _endGameWindow;
    [SerializeField] private Button _adButton;
    [SerializeField] private Timer _timer;

    public void OnRestartClick()
    {
        _timer.enabled = true;
        _board.Restart();
        _endGameWindow.SetActive(false);
        _adButton.interactable = true;
    }
}
