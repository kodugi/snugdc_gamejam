using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private SentenceParser _sentenceParser;
    private ButtonContainer _buttonContainer;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _sentenceParser = GetComponent<SentenceParser>();

        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            _allItems.Add(new ItemData { itemName = type.ToString(), type = type });
        }
    }

    private int _currentPlayer = 0; // 0 for Player 1, 1 for Player 2
    private int _remainingChoices = 2;
    private GameState _currentState = GameState.GameStart;
    private SentenceData _currentSentenceData;
    private List<(int, int)> _correctWordPositions = new List<(int, int)>();
    private List<(int, int)> _incorrectWordPositions = new List<(int, int)>();
    private List<Player> Players = new List<Player> { new Player { playerId = 0 }, new Player { playerId = 1 } };
    private List<ItemData> _allItems = new List<ItemData>();
    private Dictionary<ItemData, int> _usedItems = new Dictionary<ItemData, int>();

    // 호출 흐름: StartRound -> SelectSentence -> RevealAnswer
    public void StartRound()
    {
        _currentPlayer = 0;
        _currentState = GameState.GameStart;
    }

    public void SelectSentence()
    {
        _currentState = GameState.SelectSentence;
        // Logic for selecting a sentence goes here
        _currentSentenceData = _sentenceParser.sentenceDataList[0]; // Example: select the first sentence data
    }

    public void RevealAnswer()
    {
        _currentState = GameState.RevealAnswer;
        // Logic for revealing the answer goes here
        foreach (var pos in _correctWordPositions)
        {
            // Highlight correct words
        }
        foreach (var pos in _incorrectWordPositions)
        {
            _buttonContainer.DisableButton(pos.Item1, pos.Item2);
        }
    }

    // 호출 흐름: StartTurn ->  PlayItem -> ProcessWordChoice -> TurnEnd
    public void StartTurn()
    {
        _currentPlayer = (_currentPlayer + 1) % 2;
        _currentState = GameState.TurnStart;
        _correctWordPositions.Clear();
        _incorrectWordPositions.Clear();
        _remainingChoices = 2;
        _usedItems.Clear();
        GainItem();
    }

    public void GainItem()
    {
        foreach (var player in Players)
        {
            int totalItems = GetTotalItems(player.playerId);
            int leftCapacity = 5 - totalItems; // Assuming max capacity is 5
            for (int i = 0; i < 3 && i < leftCapacity; i++) // Example: gain 3 items
            {
                ItemData newItem = _allItems[Random.Range(0, _allItems.Count)]; // Logic to determine which item to gain
                if (player.inventory.ContainsKey(newItem))
                {
                    player.inventory[newItem]++;
                }
                else
                {
                    player.inventory[newItem] = 1;
                }
            }
        }
    }

    private int GetTotalItems(int playerId)
    {
        int total = 0;
        foreach (var itemCount in Players[playerId].inventory.Values)
        {
            total += itemCount;
        }
        return total;
    }

    public void PlayItem(ItemData item) // 아이템 선택 시 호출
    {
        _currentState = GameState.PlayItem;
        // Logic for playing an item goes here
        if (Players[_currentPlayer].inventory.ContainsKey(item) && Players[_currentPlayer].inventory[item] > 0)
        {
            Players[_currentPlayer].inventory[item]--;
            if (_usedItems.ContainsKey(item))
            {
                _usedItems[item]++;
            }
            else
            {
                _usedItems[item] = 1;
            }

            switch (item.type)
            {
                case ItemType.Transceiver:
                    (int, int) location = _correctWordPositions[Random.Range(0, _correctWordPositions.Count)];
                    // highlight word at location
                    break;
                case ItemType.MagnifyingGlass:
                    break;
                case ItemType.Americano:
                    break;
                case ItemType.AncientDocument:
                    PlayAncientDocument();
                    break;
                case ItemType.Gloves:
                    if(_usedItems[item] >= 1) // 2회 이상 사용 불가
                    {
                        break;
                    }
                    PlayGloves();
                    break;
                case ItemType.Beer:
                    break;
            }
        }
    }

    private void PlayGloves()
    {
        // 상대의 아이템 랜덤으로 탈취
        Player opponent = Players[(_currentPlayer + 1) % 2];
        List<ItemData> opponentItems = new List<ItemData>(opponent.inventory.Keys);
        if (opponentItems.Count > 0)
        {
            ItemData stolenItem = opponentItems[Random.Range(0, opponentItems.Count)];
            while(opponent.inventory[stolenItem] <= 0) // 아이템 개수가 0인 경우 다시 선택
            {
                stolenItem = opponentItems[Random.Range(0, opponentItems.Count)];
            }
                
            opponent.inventory[stolenItem]--;
            if (opponent.inventory[stolenItem] <= 0)
            {
                opponent.inventory.Remove(stolenItem);
            }

            if (Players[_currentPlayer].inventory.ContainsKey(stolenItem))
            {
                Players[_currentPlayer].inventory[stolenItem]++;
            }
            else
            {
                Players[_currentPlayer].inventory[stolenItem] = 1;
            }
        }
    }

    private void PlayAncientDocument()
    {
        // 조사 위치 두 개 공개
    }

    public void ProcessWordChoice(int row, int column) // 단어 선택 시 호출
    {
        if(_remainingChoices <= 0)
            return;
        _remainingChoices--;
        _currentState = GameState.Interpret;
        // Logic for processing the chosen word goes here
        WordData chosenWord = _currentSentenceData.sentences[row][column];
        if (chosenWord.isCorrect)
        {
            // Handle correct choice
            _correctWordPositions.Add((row, column));
            _buttonContainer.DisableColumn(column);
        }
        else
        {
            // Handle incorrect choice
            _incorrectWordPositions.Add((row, column));
        }
    }

    public void TurnEnd()
    {
        _currentState = GameState.TurnEnd;
        // Logic for ending the turn goes here
    }
}
