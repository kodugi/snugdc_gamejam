using System.Collections.Generic;
using UnityEngine;

public class Enemy: Player
{
    private GameManager _gameManager;
    private List<List<bool>> _availableWords;

    public Position getNextChoice()
    {
        // row가-1이면 skip을 의미
        int currentColumn = _gameManager.GetCurrentColumn();
        
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < _availableWords[currentColumn].Count; i++)
        {
            Debug.Log($"Availability at column {currentColumn}, row {i}: {_availableWords[currentColumn][i]}");
            if (_availableWords[currentColumn][i])
            {
                availableIndices.Add(i);
            }
        }
        Debug.Log("Remaining choices: " + _gameManager.GetRemainingChoices());
        Debug.Log("Available indices count: " + availableIndices.Count);
        int randomIndex = UnityEngine.Random.Range(0, availableIndices.Count + (_gameManager.GetRemainingChoices() >= 2 ? 0 : 1));
        // 남은 선택 횟수가 2회 이상이면 skip 불가
        // 일단은 위치를 가능한 후보군 중 랜덤으로 선택
        if(randomIndex < availableIndices.Count)
        {
            return new Position(availableIndices[randomIndex], currentColumn);
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
        _availableWords = new List<List<bool>>();
        foreach (var sentence in gameManager.GetCurrentSentenceData().sentences)
        {
            List<bool> availability = new List<bool>();
            for (int i = 0; i < sentence.Count; i++)
            {
                availability.Add(true);
            }
            _availableWords.Add(availability);
        }

    }

    public void RemoveAt(Position pos)
    {
        Debug.Log("Enemy RemoveAt at column " + pos.col + ", row " + pos.row);
        _availableWords[pos.col][pos.row] = false;
    }

    public void RemoveExceptAnswer(int column)
    {
        Debug.Log("Enemy RemoveExceptAnswer at column " + column);
        for(int row = 0; row < _availableWords[column].Count; row++)
        {
            if(_gameManager.GetCurrentSentenceData().sentences[column][row].isCorrect)
            {
                continue;
            }
            _availableWords[column][row] = false;
        }
    }
}