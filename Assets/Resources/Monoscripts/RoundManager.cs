using UnityEngine;
using System.Collections.Generic;

public class RoundManager : MonoBehaviour
{
    private GameManager _gameManager;
    private TurnManager _turnManager;

    private int _remainingRounds;
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
    private (int, int) _scores = (0, 0); // (playerScore, enemyScore)

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
        Debug.Log("Starting new round. Remaining rounds: " + _remainingRounds);
        _remainingRounds--;
        _currentColumn = 0;
        _turnManager.ResetPlayer();
        _correctColumns.Clear();
        SelectSentence();
        InitRun();
    }
    public void SelectSentence()
    {
        // Logic for selecting a sentence goes here
        _currentSentenceData = _gameManager.SentenceParser.sentenceDataList[Random.Range(0, _gameManager.SentenceParser.sentenceDataList.Count)];
        _gameManager.ButtonContainer.Init(_currentSentenceData);
        _gameManager.GetEnemy().Initialize(_gameManager, _turnManager);
    }

    public void EndRound()
    {
        if(_gameManager.GetCurrentPlayer().playerId == 0)
        {
            _scores.Item1 += 1;
        }
        else
        {
            _scores.Item2 += 1;
        }
        Debug.Log($"Scores - Player: {_scores.Item1}, Enemy: {_scores.Item2}");
        Debug.Log("Remaining Rounds: " + _remainingRounds);
        if (_remainingRounds > 0)
        {
            StartRound();
        }
        else
        {
            Debug.Log("Game Over");
            Debug.Log($"Final Scores - Player: {_scores.Item1}, Enemy: {_scores.Item2}");
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
    }
    public void InitRun()
    {
        _correctWordPositions.Clear();
        _incorrectWordPositions.Clear();
        _currentColumn = 0;
        while (_correctColumns.Contains(_currentColumn) && _currentColumn < _currentSentenceData.sentences.Count)
        {
            _currentColumn++; // 이미 맞힌 열은 건너뜀
        }
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
            _gameManager.GetEnemy().RemoveAt(pos);
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
        if(_currentColumn==_currentSentenceData.sentences.Count)
            RevealAnswer();
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
