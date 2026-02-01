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
    private SentenceData _currentSentenceData; // _currentSentenceData[a][b]: a: col, b: row
    private List<(int, int)> _correctWordPositions = new List<(int, int)>();
    private List<(int, int)> _incorrectWordPositions = new List<(int, int)>();
    private List<Player> Players = new List<Player> { new Player { playerId = 0 }, new Player { playerId = 1 } };
    private List<ItemData> _allItems = new List<ItemData>();
    private DefaultDictionary<ItemData, int> _usedItems = new DefaultDictionary<ItemData, int>();
    private bool _usedBeerLastTurn = false;

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

    public void PlayItem(ItemData item) // 아이템 선택 시 호출
    {
        _currentState = GameState.PlayItem;
        // Logic for playing an item goes here
        if (Players[_currentPlayer].inventory[item] > 0)
        {
            // 이용 가능 여부 판정
            switch (item.type)
            {
                case ItemType.Gloves:
                    if(_usedItems[item] >= 1) // 2회 이상 사용 불가
                    {
                        return;
                    }
                    break;
                case ItemType.Beer:
                    if(_usedBeerLastTurn)
                    {
                        return; // 연속 사용 불가
                    }
                    break;
            }

            Players[_currentPlayer].inventory[item]--;
            _usedItems[item]++;

            // 효과 발동
            switch (item.type)
            {
                case ItemType.Transceiver:
                    PlayTransceiver();
                    // highlight word at location
                    break;
                case ItemType.MagnifyingGlass:
                    // 선택한 위치가 정답인지 공개
                    break;
                case ItemType.Americano:
                    // 추가 턴 진행, 별도 함수 발동은 없음
                    break;
                case ItemType.AncientDocument:
                    PlayAncientDocument();
                    break;
                case ItemType.Gloves:
                    PlayGloves();
                    break;
                case ItemType.Beer:
                    // 이번 턴에 무조건 정답을 처리해야 함, 별도 함수 발동은 없음
                    break;
            }
        }
    }

    private void PlayTransceiver()
    {
        // 무작위 열의 정답 공개
        List<(int, int)> correctLocations = new List<(int, int)>();
        for (int col = 0; col < _currentSentenceData.sentences.Count; col++)
        {
            for (int row = 0; row < _currentSentenceData.sentences[col].Count; row++)
            {
                WordData wordData = _currentSentenceData.sentences[col][row];
                if (wordData.type == WordType.Conjunction && wordData.isCorrect)
                {
                    correctLocations.Add((row, col));
                    break;
                }
            }
        }

        int randomIndex = Random.Range(0, correctLocations.Count);
        WordData wordData = _currentSentenceData.sentences[correctLocations[randomIndex].Item2][correctLocations[randomIndex].Item1];
        // Highlight the word at correctLocations[randomIndex]
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

            Players[_currentPlayer].inventory[stolenItem]++;
        }
    }

    private void PlayAncientDocument()
    {
        // 조사 위치 두 개 공개
        List<int> conjunctionColumns = new List<int>();
        for (int col = 0; col < _currentSentenceData.sentences.Count; col++)
        {
            for (int row = 0; row < _currentSentenceData.sentences[col].Count; row++)
            {
                WordData wordData = _currentSentenceData.sentences[col][row];
                if (wordData.type == WordType.Conjunction && wordData.isCorrect)
                {
                    conjunctionColumns.Add(col);
                    break;
                }
            }
        }

        int revealCount = Mathf.Min(2, conjunctionColumns.Count);
        for (int i = 0; i < revealCount; i++)
        {
            int randomIndex = Random.Range(0, conjunctionColumns.Count);
            int colToReveal = conjunctionColumns[randomIndex];
            conjunctionColumns.RemoveAt(randomIndex);

            for (int row = 0; row < _currentSentenceData.sentences[colToReveal].Count; row++)
            {
                WordData wordData = _currentSentenceData.sentences[colToReveal][row];
                if (wordData.type == WordType.Conjunction && wordData.isCorrect)
                {
                    // Highlight the column
                    break;
                }
            }
        }
    }

    public void ProcessWordChoice(int row, int column) // 단어 선택 시 호출
    {
        if(_remainingChoices <= 0)
            return;
        _remainingChoices--;
        _currentState = GameState.Interpret;
        // Logic for processing the chosen word goes here
        WordData chosenWord = _currentSentenceData.sentences[column][row];
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
        if(_usedItems[new ItemData { type = ItemType.Americano }] > 0)
        {
            _usedBeerLastTurn = true;
            return; // 추가 턴이므로 플레이어 변경 안함
        }
        _usedBeerLastTurn = false;
        _currentPlayer = (_currentPlayer + 1) % 2;
    }
}
