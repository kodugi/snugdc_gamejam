using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class TurnManager : MonoBehaviour
{
    private GameManager _gameManager;
    private RoundManager _roundManager;

    private int _currentPlayer = 0;
    public int RemainingChoices
    {
        get => __remainingChoices;
        private set
        {
            __remainingChoices = value;
            _gameManager.UIManager.UpdateRemainingChoices(value);
            _gameManager.UIManager.UpdateSkipButton(value);
        }
    }

    public void AddRemainingChoices(int amount)
    {
        RemainingChoices += amount;
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
        RemainingChoices = 2;
        _usedItems.Clear();
        _gameManager.GainItem();
        _gameManager.ButtonContainer.UnHighLightAll();
        _gameManager.ButtonContainer.HighLightColumn(_roundManager.CurrentColumn);
        _gameManager.UIManager.UpdateRemainingChoices(RemainingChoices);
        if (_currentPlayer == 1)
        {
            Debug.Log("Enemy's Turn");
            PlayEnemyTurn();
        }
        else
        {
            _gameManager.UIManager.DisDisableAll();
        }
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

    private async void PlayEnemyTurn()
    {
        await GameManager.Instance.GetEnemy().PlayTurnAsync();
    }
    public void ProcessWordChoice(int row, int column)
    {
        if (column != _roundManager.CurrentColumn)
        {
            Debug.Log("clicked:" + column + ",need:" + _roundManager.CurrentColumn);
            return;
        }
        if (RemainingChoices <= 0)
            return;
        RemainingChoices--;
        WordData chosenWord = _roundManager.CurrentSentenceData.sentences[column][row];
        if (chosenWord.isCorrect)
        {
            _gameManager.PlaySound(AudioType.Correct);
            _gameManager.ButtonContainer.DisableColumn(column);
            _roundManager.CorrectWordPositionsAdd(new Position(row, column));
        }
        else
        {
            _gameManager.PlaySound(AudioType.Incorrect);
            _gameManager.ButtonContainer.DisableButton(row, column);
            _roundManager.IncorrectWordPositionsAdd(new Position(row, column));
        }

        _roundManager.AdvanceColumn();
        
        if(RemainingChoices==0)
        {
            TurnEnd();
        }
    }

    public void TurnEnd()
    {
        if (RemainingChoices >= 2) return;
        /*if (_usedItems[ItemType.Beer] > 0 && RemainingChoices > 0)
        {
            return;
        }*/

        RemainingChoices = 2;
        _currentPlayer = (_currentPlayer + 1) % 2;
        
        _gameManager.UIManager.DisableAll();
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
