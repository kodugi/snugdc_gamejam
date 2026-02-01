using UnityEngine;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    private GameManager _gameManager;
    private RoundManager _roundManager;

    private int _currentPlayer = 0;
        private int _remainingChoices
    {
        get => __remainingChoices;
        set
        {
            __remainingChoices = value;
            _gameManager.UIManager.UpdateRemainingChoices(value);
            _gameManager.UIManager.UpdateSkipButton(value);
        }
    }
    private int __remainingChoices = 2;
    private DefaultDictionary<ItemType, int> _usedItems = new DefaultDictionary<ItemType, int>();
    private bool _usedAmericanoLastTurn = false;

    public int CurrentPlayer => _currentPlayer;

    private void Awake()
    {
        _gameManager = GetComponent<GameManager>();
        _roundManager = GetComponent<RoundManager>();
    }

    public void ResetPlayer()
    {
        _currentPlayer = 0;
    }

    public void StartTurn()
    {
        _remainingChoices = 2;
        _usedItems.Clear();
        _gameManager.ButtonContainer.UnHighLightAll();
        _gameManager.ButtonContainer.HighLightColumn(_roundManager.CurrentColumn);
        _gameManager.UIManager.UpdateRemainingChoices(_remainingChoices);
        GainItem();
    }

    public void GainItem()
    {
        foreach (var player in _gameManager.GetPlayers())
        {
            int totalItems = GetTotalItems(player.playerId);
            int leftCapacity = 5 - totalItems; // Assuming max capacity is 5
            for (int i = 0; i < 3 && i < leftCapacity; i++) // Example: gain 3 items
            {
                ItemType newItem = _gameManager.GetAllItems()[Random.Range(0, _gameManager.GetAllItems().Count)]; // Logic to determine which item to gain
                player.inventory[newItem]++;
            }
        }
    }

    private int GetTotalItems(int playerId)
    {
        int total = 0;
        foreach (var itemCount in _gameManager.GetPlayers()[playerId].inventory.Values)
        {
            total += itemCount;
        }
        return total;
    }

    public void PlayItem(ItemType item)
    {
        if (_gameManager.GetPlayers()[_currentPlayer].inventory[item] > 0)
        {
            switch (item)
            {
                case ItemType.Gloves:
                    if (_usedItems[item] >= 1) return;
                    break;
                case ItemType.Americano:
                    if (_usedAmericanoLastTurn) return;
                    break;
            }

            _gameManager.GetPlayers()[_currentPlayer].inventory[item]--;
            _usedItems[item]++;

            _gameManager.GetItemStrategy(item).Use(_gameManager);
        }
    }

    public void ProcessWordChoice(int row, int column)
    {
        if (column != _roundManager.CurrentColumn)
        {
            Debug.Log("clicked:" + column + ",need:" + _roundManager.CurrentColumn);
            return;
        }
        if (_remainingChoices <= 0)
            return;
        _remainingChoices--;
        Debug.Log("select " + row + ":" + column);
        WordData chosenWord = _roundManager.CurrentSentenceData.sentences[column][row];
        Debug.Log("Chosen word: " + chosenWord.word + ", isCorrect: " + chosenWord.isCorrect);
        if (chosenWord.isCorrect)
        {
            Debug.Log("Correct choice!");
            _gameManager.ButtonContainer.DisableColumn(column);
            _roundManager.CorrectWordPositionsAdd(new Position(row, column));
        }
        else
        {
            Debug.Log("Incorrect choice!");
            _gameManager.ButtonContainer.DisableButton(row, column);
            _roundManager.IncorrectWordPositionsAdd(new Position(row, column));
        }

        _roundManager.AdvanceColumn();

        _gameManager.ButtonContainer.UnHighLightAll();
        if (_remainingChoices > 0)
        {
            _gameManager.ButtonContainer.HighLightColumn(_roundManager.CurrentColumn);
        }
        else
        {
            TurnEnd();
        }
    }

    public void TurnEnd()
    {
        if (_remainingChoices == 2) return;
        Debug.Log("turnEnded");
        if (_usedItems[ItemType.Beer] > 0 && _remainingChoices > 0)
        {
            return;
        }

        _remainingChoices = 2;
        if (_usedItems[ItemType.Americano] > 0)
        {
            _usedAmericanoLastTurn = true;
            return;
        }
        else
        {
            _usedAmericanoLastTurn = false;
            _currentPlayer = (_currentPlayer + 1) % 2;
        }

        if (_roundManager.CurrentColumn >= _roundManager.CurrentSentenceData.sentences.Count)
        {
            _roundManager.RevealAnswer();
        }
        else
        {
            StartTurn();
        }
    }
}
