using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Controller : MonoBehaviour
{
    [SerializeField] private Board _board;

    private Gem _otherGem;
    private Vector2 _firstTouchPos;
    private Vector2 _lastTouchPos;
    private float _swipeAngel = 0;
    private Gem _currentGem;
    private float _swipeResist = 0.3f;

    public bool canSwap = true;
    private bool _isMatched;
    private void Update()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            _currentGem = null;
            _firstTouchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.GetComponent<Gem>()!=null)
                _currentGem = hit.collider.gameObject.GetComponent<Gem>();
            
        }
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended && _currentGem!=null)
        {
            _lastTouchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            if (Mathf.Abs(_lastTouchPos.y - _firstTouchPos.y) > _swipeResist || Mathf.Abs(_lastTouchPos.x - _firstTouchPos.x) > _swipeResist)
            {
                _swipeAngel = Mathf.Atan2(_lastTouchPos.y - _firstTouchPos.y, _lastTouchPos.x - _firstTouchPos.x) * 180 / Mathf.PI;
                SwipeGems();
            }
        }
    }

    private void SwipeGems()
    {
        if (canSwap)
        {
            if (_swipeAngel > -45 && _swipeAngel <= 45 && _currentGem.column < _board.boardWidth - 1)
            {
                _otherGem = _board.allGems[_currentGem.column + 1, _currentGem.row];
                _otherGem.column -= 1;
                _board.allGems[_currentGem.column + 1, _currentGem.row] = _currentGem;
                _board.allGems[_currentGem.column, _currentGem.row] = _otherGem;
                _currentGem.column += 1;
                _isMatched = _board.CheckMatches(_currentGem, false);
                if (!_isMatched)
                {
                    _otherGem.column += 1;
                    _board.allGems[_currentGem.column - 1, _currentGem.row] = _currentGem;
                    _board.allGems[_currentGem.column, _currentGem.row] = _otherGem;
                    _currentGem.column -= 1;
                }
                StartCoroutine(AnimateSwap());
            }
            else if (_swipeAngel > 45 && _swipeAngel <= 135 && _currentGem.row < _board.boardHeight - 1)
            {
                _otherGem = _board.allGems[_currentGem.column, _currentGem.row + 1];
                _otherGem.row -= 1;
                _board.allGems[_currentGem.column, _currentGem.row + 1] = _currentGem;
                _board.allGems[_currentGem.column, _currentGem.row] = _otherGem;
                _currentGem.row += 1;
                _isMatched = _board.CheckMatches(_currentGem, false);
                if (!_isMatched)
                {
                    _otherGem.row += 1;
                    _board.allGems[_currentGem.column, _currentGem.row - 1] = _currentGem;
                    _board.allGems[_currentGem.column, _currentGem.row] = _otherGem;
                    _currentGem.row -= 1;
                }
                StartCoroutine(AnimateSwap());
            }
            else if ((_swipeAngel > 135 || _swipeAngel <= -135) && _currentGem.column > 0)
            {
                _otherGem = _board.allGems[_currentGem.column - 1, _currentGem.row];
                _otherGem.column += 1;
                _board.allGems[_currentGem.column - 1, _currentGem.row] = _currentGem;
                _board.allGems[_currentGem.column, _currentGem.row] = _otherGem;
                _currentGem.column -= 1;
                _isMatched = _board.CheckMatches(_currentGem, false);
                if (!_isMatched)
                {
                    _otherGem.column -= 1;
                    _board.allGems[_currentGem.column + 1, _currentGem.row] = _currentGem;
                    _board.allGems[_currentGem.column, _currentGem.row] = _otherGem;
                    _currentGem.column += 1;
                }
                StartCoroutine(AnimateSwap());
            }
            else if (_swipeAngel < -45 && _swipeAngel >= -135 && _currentGem.row > 0)
            {
                _otherGem = _board.allGems[_currentGem.column, _currentGem.row - 1];
                _otherGem.row += 1;
                _board.allGems[_currentGem.column, _currentGem.row - 1] = _currentGem;
                _board.allGems[_currentGem.column, _currentGem.row] = _otherGem;
                _currentGem.row -= 1;
                _isMatched = _board.CheckMatches(_currentGem, false);
                if (!_isMatched)
                {
                    _otherGem.row -= 1;
                    _board.allGems[_currentGem.column, _currentGem.row + 1] = _currentGem;
                    _board.allGems[_currentGem.column, _currentGem.row] = _otherGem;
                    _currentGem.row += 1;
                }
                StartCoroutine(AnimateSwap());
            }
        }
        
    }

    private IEnumerator AnimateSwap()
    {
        Vector2 currentGemPosition = _currentGem.gameObject.transform.position;
        Sequence simultaneousAnimations = DOTween.Sequence();
        simultaneousAnimations.Append(_currentGem.gameObject.transform.DOMove(_otherGem.gameObject.transform.position, 0.3f));
        simultaneousAnimations.Join(_otherGem.gameObject.transform.DOMove(currentGemPosition, 0.3f));
        simultaneousAnimations.Play();
        yield return new WaitForSeconds(0.3f);
        _isMatched = _board.CheckMatches(_currentGem, true);
        if (!_isMatched)
        {
            simultaneousAnimations = DOTween.Sequence();
            simultaneousAnimations.Append(_otherGem.gameObject.transform.DOMove(_currentGem.gameObject.transform.position, 0.3f));
            simultaneousAnimations.Join(_currentGem.gameObject.transform.DOMove(currentGemPosition, 0.3f));
            simultaneousAnimations.Play();
            yield return new WaitForSeconds(0.3f);
        }

    }
}
