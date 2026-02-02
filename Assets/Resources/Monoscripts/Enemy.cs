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

    public ItemType UseItem()
    {
        // 랜덤으로 아이템 사용, 선택하지 않은 경우 None 반환
        // 실제 아이템 사용 처리는 TurnManager의 PlayItem에서 수행
        // UseItem이 None을 반환할 때까지 반복 호출, 사용 불가능한 아이템을 선택한 경우 다시 호출
        List<ItemType> availableItems = new List<ItemType>();
        foreach(var item in inventory.Keys)
        {
            if(inventory[item] > 0)
            {
                availableItems.Add(item);
            }
        }

        if(availableItems.Count == 0)
        {
            return ItemType.None;
        }

        int randomIndex = UnityEngine.Random.Range(0, availableItems.Count + 1);
        if (randomIndex == availableItems.Count)
        {
            return ItemType.None;
        }
        return availableItems[randomIndex];
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