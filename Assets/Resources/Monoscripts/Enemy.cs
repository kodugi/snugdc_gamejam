using System.Collections.Generic;

public class Enemy: Player
{
    public int playerId = 1;
    private GameManager _gameManager;
    private SentenceData _wordCandidates;

    public Position getNextChoice()
    {
        // row가-1이면 skip을 의미
        var currentSentenceData = _gameManager.GetCurrentSentenceData();
        int currentColumn = _gameManager.GetCurrentColumn();
        int randomIndex = UnityEngine.Random.Range(0, _wordCandidates.sentences[currentColumn].Count + (_gameManager.GetRemainingChoices() >= 2 ? 0 : 1));
        // 남은 선택 횟수가 2회 이상이면 skip 불가
        // 일단은 위치를 가능한 후보군 중 랜덤으로 선택
        if(randomIndex < _wordCandidates.sentences[currentColumn].Count)
        {
            return new Position(randomIndex, currentColumn);
        }

        return new Position(-1, currentColumn);
    }

    public void Initialize(GameManager gameManager)
    {
        _gameManager = gameManager;
        _wordCandidates = gameManager.GetCurrentSentenceData();
    }

    public void RemoveAt(Position pos)
    {
        _wordCandidates.sentences[pos.col].RemoveAt(pos.row);
    }

    public void RemoveWord(WordData wordData, int column)
    {
        _wordCandidates.sentences[column].RemoveAll(wd => wd.word == wordData.word);
    }

    public void RemoveExceptAnswer(int column)
    {
        _wordCandidates.sentences[column].RemoveAll(wd => !wd.isCorrect);
    }
}