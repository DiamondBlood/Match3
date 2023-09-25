using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Image _timer;
    [SerializeField] private GameObject _endGameWindow;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Board _board;
    [SerializeField] private TextMeshProUGUI _currentScore;
    [SerializeField] private TextMeshProUGUI _highestScore;

    private float _maxTime = 30f;
    private float _leftTime;
    public void Initialize()
    {
        _timer.fillAmount = 1;
        _leftTime = _maxTime;
        _timerText.text = _leftTime.ToString() + " ñ.";
    }

    public void AdWatched()
    {
        _leftTime = 10f;
        _board.StartCoroutine("DecreaseRowCo");
    }
    private void Update()
    {
        _leftTime -= Time.deltaTime;
        _timerText.text = Mathf.Round(_leftTime).ToString() + " c.";
        _timer.fillAmount = _leftTime / _maxTime;
        if (_leftTime <= 0)
        {
            _leftTime = 0;
            _currentScore.text = _board.GetScore().ToString();

            if (DataManager.LoadHighestScore() < _board.GetScore())
                DataManager.SaveHighestScore(_board.GetScore());

            _highestScore.text = DataManager.LoadHighestScore().ToString();
            _board.StopAllCoroutines();
            _endGameWindow.SetActive(true);
            this.enabled = false;
        }
    }
}
