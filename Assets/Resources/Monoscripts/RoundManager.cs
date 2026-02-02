using UnityEngine;
using System.Collections.Generic;

public class RoundManager : MonoBehaviour
{
    private GameManager _gameManager;
    private TurnManager _turnManager;

    private int _remainingRounds = 3;
    private SentenceData _currentSentenceData;
    private List<Position> _correctWordPositions = new List<Position>();
    private List<Position> _incorrectWordPositions = new List<Position>();
    private int _currentColumn = 0;
    private HashSet<int> _correctColumns = new HashSet<int>();

    public SentenceData CurrentSentenceData => _currentSentenceData;
    public int CurrentColumn => _currentColumn;
    public List<Position> CorrectWordPositions => _correctWordPositions;
    public List<Position> IncorrectWordPositions => _incorrectWordPositions;
    public HashSet<int> CorrectColumns => _correctColumns;

    private void Awake()
    {
        _gameManager = GetComponent<GameManager>();
        _turnManager = GetComponent<TurnManager>();
    }

    public void StartGame()
    {
        _remainingRounds = 5;
        StartRound();
    }

    public void StartRound()
    {
        _remainingRounds--;
        _currentColumn = 0;
        _turnManager.ResetPlayer();
        _correctColumns.Clear();
        SelectSentence();
        StartRun();
    }

    public void SelectSentence()
    {
        // Logic for selecting a sentence goes here
        _currentSentenceData = _gameManager.SentenceParser.sentenceDataList[Random.Range(0, _gameManager.SentenceParser.sentenceDataList.Count)];
        _gameManager.ButtonContainer.Init(_currentSentenceData);
        _gameManager.GetEnemy().Initialize(_gameManager);
    }

    public void EndRound()
    {
        if (_remainingRounds > 0)
        {
            StartRound();
        }
        else
        {
            Debug.Log("Game Over");
        }
    }

    public void StartRun()
    {
        _correctWordPositions.Clear();
        _incorrectWordPositions.Clear();
        _currentColumn = 0;
        while (_correctColumns.Contains(_currentColumn) && _currentColumn < _currentSentenceData.sentences.Count)
        {
            _currentColumn++; // 이미 맞힌 열은 건너뜀
        }
        Debug.Log("StartRun at column " + _currentColumn);
        _turnManager.StartTurn();
    }

    public void RevealAnswer()
    {
        foreach (var pos in _correctWordPositions)
        {
            Debug.Log("Revealing answer at " + pos.row + ", " + pos.col);
            _correctColumns.Add(pos.col);
            _gameManager.GetEnemy().RemoveExceptAnswer(pos.col); // 적의 후보군에서도 정답만 남김
            _gameManager.ButtonContainer.DisableColumn(pos.col);
            _gameManager.ButtonContainer.HighlightButton(pos.row, pos.col);
        }
        foreach (var pos in _incorrectWordPositions)
        {
            _gameManager.GetEnemy().RemoveWord(_currentSentenceData.sentences[pos.col][pos.row], pos.col); // 적의 후보군에서 틀린 단어 제거
            _gameManager.ButtonContainer.DisableButton(pos.row, pos.col);
        }
        EndRun();
    }

    public void EndRun()
    {
        Debug.Log($"EndRun. Correct columns so far: {_correctColumns.Count}");
        if (_correctColumns.Count < _currentSentenceData.sentences.Count)
        {
            StartRun();
        }
        else
        {
            EndRound();
        }
    }

    public void AdvanceColumn()
    {
        do
        {
            _currentColumn++;
        }
        while (_correctColumns.Contains(_currentColumn) && _currentColumn < _currentSentenceData.sentences.Count);
        if(_currentColumn==_currentSentenceData.sentences.Count) _currentColumn = 0;
        while (_correctColumns.Contains(_currentColumn) && _currentColumn < _currentSentenceData.sentences.Count)
        {
            _currentColumn++;
        }
    }

    public void CorrectWordPositionsAdd(Position position)
    {
        _correctWordPositions.Add(position);
        _correctColumns.Add(position.col);
    }public void IncorrectWordPositionsAdd(Position position)
    {
        _incorrectWordPositions.Add(position);
    }
}
