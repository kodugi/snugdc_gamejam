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
        _gameManager.UIManager.PlayerSpriteSetArrow(_currentPlayer);
        if(_currentPlayer==0)
        {
            _gameManager.GainItem();
        }
        _gameManager.SetDrankBeer(false);
        _gameManager.ButtonContainer.UnHighLightAll();
        _gameManager.UIManager.UpdateRemainingChoices(RemainingChoices);
        if (_currentPlayer == 1)
        {
            
            _gameManager.ShowInfoUIManager("CPU 의 턴");
            _gameManager.ButtonContainer.UnUnHighLightColumn(_roundManager.CurrentColumn);
            PlayEnemyTurn();
        }
        else
        {
            _gameManager.ShowInfoUIManager("당신 의 턴");
            _gameManager.ButtonContainer.HighLightColumn(_roundManager.CurrentColumn);
            _gameManager.UIManager.DisDisableAll();
        }
    }

    public void GainItem()
    {
        foreach (var player in _gameManager.GetPlayers())
        {
            int totalItems = GetTotalItems(player.playerId);
            int leftCapacity = 3 - totalItems; // Assuming max capacity is 3
            for (int i = 0; i < 1 && i < leftCapacity; i++) // Example: gain 1 item
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
                    if (_gameManager.GetOpponent().GetTotalItems() == 0) return; // 상대방이 아이템이 없으면 사용 불가
                    break;
                case ItemType.Americano:
                    if (_usedAmericanoLastTurn) return;
                    break;
            }

            _gameManager.GetPlayers()[_currentPlayer].inventory[item]--;
            _usedItems[item]++;
            _gameManager.SoundManager.PlaySound(AudioType.ItemUse);

            string message = "";
            if(_currentPlayer == 0) // 플레이어가 아이템을 사용할 때만 메시지 표시
            {
                switch (item)
                {
                    // 나머지 아이템은 자체적으로 메시지가 나오므로 표시할 필요 없음
                    case ItemType.Americano:
                        message = "플레이어가 단어를 한 번 더 고를 수 있도록 합니다.";
                        break;
                    case ItemType.Beer:
                        message = "틀릴 때까지 단어를 선택합니다. 틀릴 시, 패배합니다.";
                        break;
                    case ItemType.Gloves:
                        message = "상대방의 무작위 아이템을 훔쳐옵니다.";
                        break;
                }
            }
            else
            {
                switch (item)
                {
                    // 모든 아이템의 메시지 표시
                    case ItemType.AncientDocument:
                        message = "CPU가 단어의 힌트를 확인합니다.";
                        break;
                    case ItemType.Americano:
                        message = "CPU가 단어를 한 번 더 고를 수 있도록 합니다.";
                        break;
                    case ItemType.Beer:
                        message = "CPU가 맥주를 마십니다.";
                        break;
                    case ItemType.Gloves:
                        message = "CPU가 상대방의 무작위 아이템을 훔쳐옵니다.";
                        break;
                    case ItemType.MagnifyingGlass:
                        message = "CPU가 단어의 힌트를 확인합니다.";
                        break;
                    case ItemType.Transceiver:
                        message = "CPU가 단어의 힌트를 확인합니다.";
                        break;
                }
            }
            

            if (message != "")
            {
                _gameManager.ShowInfoUIManager(message);
            }
            
            
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
        if (!_gameManager.HasDrankBeer())
        {
            RemainingChoices--; // 맥주를 마시면 선택 횟수가 줄어들지 않음
        }
        
        WordData chosenWord = _roundManager.CurrentSentenceData.sentences[column][row];
        if (chosenWord.isCorrect)
        {
            _gameManager.SoundManager.PlaySound(AudioType.Correct);
            _gameManager.ButtonContainer.DisableColumn(column);
            _roundManager.CorrectWordPositionsAdd(new Position(row, column));
        }
        else
        {
            if(_gameManager.HasDrankBeer())
            {
                // 해당 라운드 패배 처리
                _roundManager.ChooseWinner(_gameManager.GetOpponent().playerId);
                return;
            }
            _gameManager.SoundManager.PlaySound(AudioType.Incorrect);
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
        _gameManager.UIManager.PlayerSpriteResetArrow();
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
