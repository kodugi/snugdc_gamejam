using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;

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
    private List<(int, int)> _correctWordPositions = new List<(int, int)>(); // (row, col)
    private List<(int, int)> _incorrectWordPositions = new List<(int, int)>(); // (row, col)
    private List<Player> Players = new List<Player> { new Player { playerId = 0 }, new Player { playerId = 1 } };
    private List<ItemData> _allItems = new List<ItemData>();
    private DefaultDictionary<ItemData, int> _usedItems = new DefaultDictionary<ItemData, int>();
    private bool _usedAmericanoLastTurn = false;
    private int _currentColumn = 0;

    // 호출 흐름: StartRound -> SelectSentence -> RevealAnswer
    // Round: 한 문장을 가지고 진행되는 매 판, Run: 맨 끝 단어까지의 한 번의 진행, Turn: 한 플레이어가 아이템 사용 및 단어 선택을 하는 한 차례 행동
    public void StartRound()
    {
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
            // Highlight correct words
            ButtonContainer.Instance.HighlightButton(pos.Item1,pos.Item2);
            ButtonContainer.Instance.DisableColumn(pos.Item2);
        }
        foreach (var pos in _incorrectWordPositions)
        {
            ButtonContainer.Instance.DisableButton(pos.Item1, pos.Item2);
        }
        EndRun();
    }

    public void EndRun()
    {
        // Logic for ending the run goes here
        // Prepare for the next round or end the game
        StartRun();
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
            switch (item.type)
            {
                case ItemType.Transceiver:
                    PlayTransceiver();
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
        List<(int, int)> correctLocations = new List<(int, int)>(); // (row, col)
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
        // Highlight the word at correctLocations[randomIndex]
        _buttonContainer.HighlightButton(correctLocations[randomIndex].Item2, correctLocations[randomIndex].Item1);
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
                    _buttonContainer.HighLightColumn(colToReveal);
                    break;
                }
            }
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
            _correctWordPositions.Add((row, column));
        }
        else
        {
            // Handle incorrect choice
            ButtonContainer.Instance.DisableButton(row,column);
            _incorrectWordPositions.Add((row, column));
        }
        _currentColumn++;
        ButtonContainer.Instance.UnHighLightAll();
        ButtonContainer.Instance.HighLightColumn(_currentColumn);
    }

    public void TurnEnd()
    {
        if(_usedItems[new ItemData { type = ItemType.Beer }] > 0 && _remainingChoices > 0)
        {
            return; // 이번 턴에 무조건 정답을 처리해야 함
        }

        _currentState = GameState.TurnEnd;
        // Logic for ending the turn goes here
        if(_usedItems[new ItemData { type = ItemType.Americano }] > 0)
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
    }
}
