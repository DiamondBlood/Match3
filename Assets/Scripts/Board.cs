using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;


public class Board : MonoBehaviour
{
    [SerializeField] private GameObject[] _gemPrefab;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Timer _timer;
    [SerializeField] private Controller _controller;

    private int _score;
    private List<GameObject> _gemsInRowToDestroyList;
    private List<GameObject> _gemsInColumnToDestroyList;
    public int boardWidth { get; private set; }
    public int boardHeight { get; private set; }

    public Gem[,] allGems;

    public int GetScore() => _score;
    void Start()
    {
        Application.targetFrameRate = 60;
        boardHeight = 7;
        boardWidth = 7;
        _gemsInRowToDestroyList = new List<GameObject>();
        _gemsInColumnToDestroyList = new List<GameObject>();
        allGems = new Gem[boardWidth, boardHeight];
        SetUp();
    }

    public void Restart()
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                Destroy(allGems[i, j].gameObject);
                allGems[i, j] = null;
            }
        }
       SetUp();
    }

    private void SetUp()
    {
        _score = 0;
        _scoreText.text = "0";
        _gemsInColumnToDestroyList.Clear();
        _gemsInRowToDestroyList.Clear();
        
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                int typeOfGem = Random.Range(0, _gemPrefab.Length);
                while (MatchesAt(i,j, _gemPrefab[typeOfGem].GetComponent<Gem>()))
                {
                    typeOfGem = Random.Range(0, _gemPrefab.Length);
                }
                GameObject gem = Instantiate(_gemPrefab[typeOfGem], new Vector2(i*0.6f, j*0.6f), Quaternion.identity);
                gem.transform.parent = transform;
                gem.name = "(" + i + "," + j + ")";
                gem.GetComponent<Gem>().row = j;
                gem.GetComponent<Gem>().column = i;
                allGems[i, j] = gem.GetComponent<Gem>();
            }
        }
        _timer.Initialize();
    }

    private void CheckForMatchesOnBoard()
    {
        for (int column = 0; column < boardWidth; column++)
        {
            for (int row = 0; row < boardHeight; row++)
            {
                Gem currentGem = allGems[column, row];
                if (currentGem != null)
                {
                    if (column == 6 && row == 6)
                        _controller.canSwap = true;
                    _gemsInColumnToDestroyList.Clear();
                    _gemsInRowToDestroyList.Clear();
                    int horizontalMatch = CheckHorizontalMatches(currentGem);
                    int verticalMatch = CheckVerticalMatches(currentGem);
                    if (horizontalMatch >= 3 || verticalMatch >= 3)
                    {
                        StartCoroutine(DestroyAnimation(currentGem));
                        return;
                    }
                }
            }
        }
    }

    private bool MatchesAt(int column, int row, Gem gem)
    {
        if (column > 1 && row > 1)
        {
            if (allGems[column - 1, row].typeOfGem == gem.typeOfGem && allGems[column - 2, row].typeOfGem == gem.typeOfGem)
                return true;
            if (allGems[column, row - 1].typeOfGem == gem.typeOfGem && allGems[column, row - 2].typeOfGem == gem.typeOfGem)
                return true;

        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
                if (allGems[column, row - 1].typeOfGem == gem.typeOfGem && allGems[column, row - 2].typeOfGem == gem.typeOfGem)
                    return true;
            if (column > 1)
                if (allGems[column - 1, row].typeOfGem == gem.typeOfGem && allGems[column - 2, row].typeOfGem == gem.typeOfGem)
                    return true;

        }
        return false;
    }

    public bool CheckMatches(Gem currentGem, bool isChecked)
    {
        _controller.canSwap = false;
        _gemsInColumnToDestroyList.Clear();
        _gemsInRowToDestroyList.Clear();
        int horizontalMatches = CheckHorizontalMatches(currentGem);
        int verticalMatches = CheckVerticalMatches(currentGem);
        if (isChecked)
        {
            StartCoroutine(DestroyAnimation(currentGem));
        }
        return horizontalMatches >= 3 || verticalMatches >= 3;
    }
    
    private IEnumerator DestroyAnimation(Gem currentGem)
    {
        if (_gemsInColumnToDestroyList.Count >= 3 && _gemsInColumnToDestroyList.Count > _gemsInRowToDestroyList.Count)
        {
            Sequence simultaneousAnimations = DOTween.Sequence();
            for (int i = 0; i < _gemsInColumnToDestroyList.Count; i++)
            {
                simultaneousAnimations.Insert(0, _gemsInColumnToDestroyList[i].transform.DOScale(Vector2.zero, 0.2f));
            }
            simultaneousAnimations.Play();
            yield return new WaitForSeconds(0.3f);
            for (int i = 0; i < _gemsInColumnToDestroyList.Count; i++)
            {
                Destroy(_gemsInColumnToDestroyList[i]);
                int index = _gemsInColumnToDestroyList[i].GetComponent<Gem>().column;
                allGems[index, currentGem.row] = null;
            }
            _score += _gemsInColumnToDestroyList.Count * (_gemsInColumnToDestroyList.Count - 2);
            _scoreText.text = _score.ToString();
        }
        else if (_gemsInRowToDestroyList.Count >= 3 && _gemsInRowToDestroyList.Count > _gemsInColumnToDestroyList.Count)
        {
            Sequence simultaneousAnimations = DOTween.Sequence();
            for (int i = 0; i < _gemsInRowToDestroyList.Count; i++)
            {
                simultaneousAnimations.Insert(0, _gemsInRowToDestroyList[i].transform.DOScale(Vector2.zero, 0.2f));
            }
            simultaneousAnimations.Play();
            yield return new WaitForSeconds(0.3f);
            for (int i = 0; i < _gemsInRowToDestroyList.Count; i++)
            {

                Destroy(_gemsInRowToDestroyList[i]);
                int index = _gemsInRowToDestroyList[i].GetComponent<Gem>().row;
                allGems[currentGem.column, index] = null;
            }
            _score += _gemsInRowToDestroyList.Count * (_gemsInRowToDestroyList.Count - 2);
            _scoreText.text = _score.ToString();
        }

       StartCoroutine(DecreaseRowCo());
    }
    private int CheckHorizontalMatches(Gem currentGem)
    {
        int matches = 1;
        int targetType = currentGem.typeOfGem;
        int column = currentGem.column;
        int row = currentGem.row;
        _gemsInColumnToDestroyList.Add(allGems[column, row].gameObject);

        // Проверка влево
        for (int c = column - 1; c >= 0; c--)
        {
            if (allGems[c, row].typeOfGem == targetType)
            {
                _gemsInColumnToDestroyList.Add(allGems[c, row].gameObject);
                matches++;
            }
            else
            {
                break;
            }
        }

        // Проверка вправо
        for (int c = column + 1; c < boardWidth; c++)
        {
            if (allGems[c, row].typeOfGem == targetType)
            {
                _gemsInColumnToDestroyList.Add(allGems[c, row].gameObject);
                matches++;
            }
            else
            {
                break;
            }
        }

        return matches;
    }

    private int CheckVerticalMatches(Gem currentGem)
    {
        int matches = 1;
        int targetType = currentGem.typeOfGem;
        int column = currentGem.column;
        int row = currentGem.row;
        _gemsInRowToDestroyList.Add(allGems[column, row].gameObject);
        // Проверка вверх
        for (int r = row - 1; r >= 0; r--)
        {
            if (allGems[column, r].typeOfGem == targetType)
            {
                _gemsInRowToDestroyList.Add(allGems[column, r].gameObject);
                matches++;
            }
            else
            {
                break;
            }
        }

        // Проверка вниз
        for (int r = row + 1; r < boardHeight; r++)
        {
            if (allGems[column, r].typeOfGem == targetType)
            {
                _gemsInRowToDestroyList.Add(allGems[column, r].gameObject);
                matches++;
            }
            else
            {
                break;
            }
        }

        return matches;
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (allGems[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allGems[i, j].transform.DOLocalMoveY((allGems[i, j].row - nullCount) * 1.5f + 0.15f, 0.4f);
                    allGems[i, j].row -= nullCount;
                    allGems[i, j - nullCount] = allGems[i, j];
                    allGems[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoardCol());
    }

    public void RefillBoard()
    {
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                if (allGems[i,j] == null)
                {
                    GameObject gem = Instantiate(_gemPrefab[Random.Range(0, _gemPrefab.Length)], new Vector2(i*0.6f, j*0.6f*1.5f), Quaternion.identity);
                    gem.transform.DOMoveY(j * 0.6f, 0.3f);
                    allGems[i,j] = gem.GetComponent<Gem>();
                    gem.transform.parent = transform;
                    gem.name = "(" + i + "," + j + ")";
                    gem.GetComponent<Gem>().row = j;
                    gem.GetComponent<Gem>().column = i;
                }
            }
        }
    }

    private IEnumerator FillBoardCol()
    {
        RefillBoard();
        yield return new WaitForSeconds(0.2f);
        CheckForMatchesOnBoard();
    }
}
