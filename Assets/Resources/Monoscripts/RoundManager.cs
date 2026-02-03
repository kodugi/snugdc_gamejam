using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    private HashSet<int> _usedSentenceIndices = new HashSet<int>();

    public SentenceData CurrentSentenceData => _currentSentenceData;
    public int CurrentColumn => _currentColumn;
    public List<Position> CorrectWordPositions => _correctWordPositions;
    public List<Position> IncorrectWordPositions => _incorrectWordPositions;
    public HashSet<int> CorrectColumns => _correctColumns;
    private (int, int) _scores = (0, 0); // (playerScore, enemyScore)
    private bool isEndRoundActive = false;
    private void Awake()
    {
        _gameManager = GetComponent<GameManager>();
        _turnManager = GetComponent<TurnManager>();
    }

    public void StartGame()
    {
        _remainingRounds = 5;
        _scores = (0, 0);
        _gameManager.UIManager.UpdateScore(_scores.Item1, _scores.Item2);
        StartRound();
    }

    public void StartRound()
    {
        Debug.Log("Starting new round. Remaining rounds: " + _remainingRounds);
        _gameManager.UIManager.PlayersSpriteReset();
        _remainingRounds--;
        _currentColumn = 0;
        _turnManager.ResetPlayer();
        _correctColumns.Clear();
        SelectSentence();
        InitRun();
    }
    public void SelectSentence()
    {
        var sentenceList = _gameManager.SentenceParser.sentenceDataList;
        if (_usedSentenceIndices.Count >= sentenceList.Count)
        {
            _usedSentenceIndices.Clear();
        }

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, sentenceList.Count);
        } while (_usedSentenceIndices.Contains(randomIndex));

        _usedSentenceIndices.Add(randomIndex);
        _currentSentenceData = sentenceList[randomIndex];

        _gameManager.ButtonContainer.Init(_currentSentenceData);
        _gameManager.GetEnemy().Initialize(_gameManager, _turnManager);
    }

    public async void EndRound()
    {
        if (isEndRoundActive) return;
        int winnerId = _gameManager.GetCurrentPlayer().playerId;
        ChooseWinner(winnerId);
    }

    public async void ChooseWinner(int winnerId)
    {
        if (isEndRoundActive) return;
        isEndRoundActive = true;
        if(winnerId == 0)
        {
            _scores.Item1 += 1;
            _gameManager.UIManager.PlayerWinRender();
        }
        else
        {
            _scores.Item2 += 1;
            _gameManager.UIManager.EnemyWinRender();
        }
        Debug.Log($"Scores - Player: {_scores.Item1}, Enemy: {_scores.Item2}");
        Debug.Log("Remaining Rounds: " + _remainingRounds);
        _gameManager.UIManager.UpdateScore(_scores.Item1, _scores.Item2);

        _gameManager.ShowInfoUIManager(winnerId == 0 ? "플레이어 승리!" : "CPU 승리!");
        if(winnerId == 0)
        {
            _gameManager.SoundManager.PlaySound(AudioType.Victory);
        }
        await Task.Delay(3000);
        isEndRoundActive = false;
        if (_scores.Item1<3 && _scores.Item2<3)
        {
            StartRound();
        }
        else
        {
            _gameManager.UIManager.SetEndUI(_scores.Item1>=3);
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
            _gameManager.ButtonContainer.UnHighLightAll();
        if(_turnManager.CurrentPlayer==0)
        {
            _gameManager.ButtonContainer.HighLightColumn(_currentColumn);
        }
        else
        {
            _gameManager.ButtonContainer.UnUnHighLightColumn(_currentColumn);
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
