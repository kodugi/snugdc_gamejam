using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private SentenceParser _sentenceParser;
    private ButtonContainer _buttonContainer;

    private Dictionary<ItemType, IItem> _itemStrategies = new Dictionary<ItemType, IItem>();

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
            _allItems.Add(type);
        }

        _itemStrategies.Add(ItemType.Transceiver, new Transceiver());
        _itemStrategies.Add(ItemType.MagnifyingGlass, new MagnifyingGlass());
        _itemStrategies.Add(ItemType.Americano, new Americano());
        _itemStrategies.Add(ItemType.AncientDocument, new AncientDocument());
        _itemStrategies.Add(ItemType.Gloves, new Gloves());
        _itemStrategies.Add(ItemType.Beer, new Beer());

        StartGame();
    }

    private int _currentPlayer = 0; // 0 for Player 1, 1 for Player 2
    private int _remainingChoices = 2;
    private GameState _currentState = GameState.GameStart;
    private SentenceData _currentSentenceData; // _currentSentenceData[a][b]: a: col, b: row
    private List<Position> _correctWordPositions = new List<Position>(); // (row, col)
    private List<Position> _incorrectWordPositions = new List<Position>(); // (row, col)
    private List<Player> Players = new List<Player> { new Player { playerId = 0 }, new Player { playerId = 1 } };
    private List<ItemType> _allItems = new List<ItemType>();
    private DefaultDictionary<ItemType, int> _usedItems = new DefaultDictionary<ItemType, int>();
    private bool _usedAmericanoLastTurn = false;
    private int _currentColumn = 0;
    private int _remainingRounds = 3;
    private HashSet<int> _correctColumns = new HashSet<int>();

    // 호출 흐름: StartRound -> SelectSentence -> RevealAnswer
    // Game: 전체 게임, Round: 한 문장을 가지고 진행되는 매 판, Run: 맨 끝 단어까지의 한 번의 진행, Turn: 한 플레이어가 아이템 사용 및 단어 선택을 하는 한 차례 행동

    public void StartGame()
    {
        _remainingRounds = 3;
        StartRound();
    }

    public void StartRound()
    {
        _remainingRounds--;
        _currentColumn = 0;
        _currentPlayer = 0;
        _currentState = GameState.GameStart;
        SelectSentence();
        StartRun();
    }

    public void SelectSentence()
    {
        _currentState = GameState.SelectSentence;
        // Logic for selecting a sentence goes here
        _currentSentenceData = _sentenceParser.sentenceDataList[0];
        ButtonContainer.Instance.Init(_currentSentenceData);// Example: select the first sentence data
    }

    public void EndRound()
    {
        // Logic for ending the round goes here
        // Prepare for the next round or end the game
        if (_remainingRounds > 0)
        {
            StartRound();
        }
        else
        {
            // End the game
            Debug.Log("Game Over");
        }
    }

    public void StartRun()
    {
        _currentColumn = 0;
        StartTurn();
    }

    public void RevealAnswer()
    {
        _currentState = GameState.RevealAnswer;
        // Logic for revealing the answer goes here
        foreach (var pos in _correctWordPositions)
        {
            _correctColumns.Add(pos.col);
            // Highlight correct words
            ButtonContainer.Instance.HighlightButton(pos.row,pos.col);
            ButtonContainer.Instance.DisableColumn(pos.col);
        }
        foreach (var pos in _incorrectWordPositions)
        {
            ButtonContainer.Instance.DisableButton(pos.row, pos.col);
        }
        EndRun();
    }

    public void EndRun()
    {
        // Logic for ending the run goes here
        if (true) // Condition to check if there are more runs in the round
        {
            StartRound();
        }
        else
        {
            EndRound();
        }
    }
    // 호출 흐름: StartTurn ->  PlayItem -> ProcessWordChoice -> TurnEnd
    public void StartTurn()
    {
        _currentState = GameState.TurnStart;
        _correctWordPositions.Clear();
        _incorrectWordPositions.Clear();
        _remainingChoices = 2;
        _usedItems.Clear();
        _buttonContainer.UnHighLightAll();
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
                ItemType newItem = _allItems[Random.Range(0, _allItems.Count)]; // Logic to determine which item to gain
                player.inventory[newItem]++;
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

    public void PlayItem(ItemType item) // 아이템 선택 시 호출
    {
        _currentState = GameState.PlayItem;
        // Logic for playing an item goes here
        if (Players[_currentPlayer].inventory[item] > 0)
        {
            // 이용 가능 여부 판정
            switch (item)
            {
                case ItemType.Gloves:
                    if(_usedItems[item] >= 1) // 2회 이상 사용 불가
                    {
                        return;
                    }
                    break;
                case ItemType.Americano:
                    if(_usedAmericanoLastTurn)
                    {
                        return; // 연속 사용 불가
                    }
                    break;
            }

            Players[_currentPlayer].inventory[item]--;
            _usedItems[item]++;

            // 효과 발동
            _itemStrategies[item].Use(this);
        }
    }

    public void ProcessWordChoice(int row, int column) // 단어 선택 시 호출
    {
        if(column != _currentColumn)
            return; // 현재 선택 가능한 열이 아님
        if(_remainingChoices <= 0)
            return;
        _remainingChoices--;
        _currentState = GameState.Interpret;
        // Logic for processing the chosen word goes here
        WordData chosenWord = _currentSentenceData.sentences[column][row];
        if (chosenWord.isCorrect)
        {
            // Handle correct choice
            ButtonContainer.Instance.DisableColumn(column);
            _correctWordPositions.Add(new Position(row, column));
        }
        else
        {
            // Handle incorrect choice
            ButtonContainer.Instance.DisableButton(row,column);
            _incorrectWordPositions.Add(new Position(row, column));
        }

        do {
            // 이미 맞힌 열은 건너뜀
            _currentColumn++;
        }
        while(_correctColumns.Contains(_currentColumn) && _currentColumn < _currentSentenceData.sentences.Count);
        
        ButtonContainer.Instance.UnHighLightAll();
        ButtonContainer.Instance.HighLightColumn(_currentColumn);
    }

    public void TurnEnd()
    {
        if(_usedItems[ItemType.Beer] > 0 && _remainingChoices > 0)
        {
            return; // 이번 턴에 무조건 정답을 처리해야 함
        }

        _currentState = GameState.TurnEnd;
        // Logic for ending the turn goes here
        if(_usedItems[ItemType.Americano] > 0)
        {
            _usedAmericanoLastTurn = true;
            return; // 추가 턴이므로 플레이어 변경 안함
        }
        else
        {
            _usedAmericanoLastTurn = false;
            _currentPlayer = (_currentPlayer + 1) % 2;
        }
        
        if(_currentColumn >= _currentSentenceData.sentences.Count)
        {
            // 모든 열을 다 선택했으므로 라운드 종료
            RevealAnswer();
        }
        else
        {
            StartTurn();
        }
    }

    public SentenceData GetCurrentSentenceData()
    {
        return _currentSentenceData;
    }

    public Player GetCurrentPlayer()
    {
        return Players[_currentPlayer];
    }

    public Player GetOpponent()
    {
        return Players[(_currentPlayer + 1) % 2];
    }
}
