using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;

[RequireComponent(typeof(SentenceParser))]
[RequireComponent(typeof(RoundManager))]
[RequireComponent(typeof(TurnManager))]
[RequireComponent(typeof(UIManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private SentenceParser _sentenceParser;
    private RoundManager _roundManager;
    private TurnManager _turnManager;
    [SerializeField] ButtonContainer _buttonContainer;
    [SerializeField] TextUIManager _textUImanager;
    private UIManager _uiManager;
    public ButtonContainer ButtonContainer => _buttonContainer;
    public SentenceParser SentenceParser => _sentenceParser;
    public UIManager UIManager => _uiManager;
    private Dictionary<ItemType, IItem> _itemStrategies = new Dictionary<ItemType, IItem>();
    private SoundManager _soundManager;
    

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
        _roundManager = GetComponent<RoundManager>();
        _turnManager = GetComponent<TurnManager>();
        _uiManager = GetComponent<UIManager>();
        _soundManager = GetComponent<SoundManager>();

        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            if (type == ItemType.None) continue;
            _allItems.Add(type);
        }

        _itemStrategies.Add(ItemType.Transceiver, new Transceiver());
        _itemStrategies.Add(ItemType.MagnifyingGlass, new MagnifyingGlass());
        _itemStrategies.Add(ItemType.Americano, new Americano());
        _itemStrategies.Add(ItemType.AncientDocument, new AncientDocument());
        _itemStrategies.Add(ItemType.Gloves, new Gloves());
        _itemStrategies.Add(ItemType.Beer, new Beer());
    }

    private void Start()
    {
        StartGame();
    }

    private GameState _currentState = GameState.GameStart;
    public List<Player> Players = new List<Player> { new Player { playerId = 0 }, new Enemy { playerId = 1 } };
    private List<ItemType> _allItems = new List<ItemType>();

    public void StartGame()
    {
        Players = new List<Player> { new Player { playerId = 0 }, new Enemy { playerId = 1 } };
        _roundManager.StartGame();
    }

    public void PlayItem(ItemType item) // 아이템 선택 시 호출
    {
        Debug.Log("use"+item);
        _turnManager.PlayItem(item);
        _uiManager.ItemUpdate(Players[0].inventory,Players[1].inventory);
    }

    public void GainItem()
    {
        _turnManager.GainItem();
        _uiManager.ItemUpdate(Players[0].inventory,Players[1].inventory);
    }
    public void ProcessWordChoice(int row, int column) // 단어 선택 시 호출
    {
        _turnManager.ProcessWordChoice(row, column);
    }

    public void TurnEnd()
    {
        _turnManager.TurnEnd();
    }

    public SentenceData GetCurrentSentenceData()
    {
        return _roundManager.CurrentSentenceData;
    }

    public Player GetCurrentPlayer()
    {
        return Players[_turnManager.CurrentPlayer];
    }

    public Player GetOpponent()
    {
        return Players[(_turnManager.CurrentPlayer + 1) % 2];
    }

    public IItem GetItemStrategy(ItemType itemType)
    {
        return _itemStrategies[itemType];
    }

    public List<Player> GetPlayers()
    {
        return Players;
    }

    public Enemy GetEnemy()
    {
        return (Enemy)Players[1];
    }

    public List<ItemType> GetAllItems()
    {
        return _allItems;
    }

    public int GetCurrentColumn()
    {
        return _roundManager.CurrentColumn;
    }

    public int GetRemainingChoices()
    {
        return _turnManager.RemainingChoices;
    }

    public HashSet<int> GetCorrectColumns()
    {
        return _roundManager.CorrectColumns;
    }

    public void BeerEffect()
    {
        _roundManager.AdvanceColumn();
    }
    public void AddRemainingChoices(int amount)
    {
        Debug.Log("Adding Remaining Choices: " + amount);
        Debug.Log("Current Remaining Choices: " + _turnManager.RemainingChoices);
        _turnManager.AddRemainingChoices(amount);
        Debug.Log("New Remaining Choices: " + _turnManager.RemainingChoices);
    }

    public void PlaySound(AudioType audioType)
    {
        _soundManager.GetComponent<AudioSource>().PlayOneShot(
            audioType == AudioType.Correct ? _soundManager.CorrectSound :
            audioType == AudioType.Incorrect ? _soundManager.IncorrectSound :
            audioType == AudioType.Victory ? _soundManager.VictorySound :
            audioType == AudioType.ButtonClick ? _soundManager.ButtonClickSound :
            audioType == AudioType.ItemUse ? _soundManager.ItemUseSound :
            null
        );
    }
    public void ShowTextUIManager(string text,Sprite sprite)
    {
        _textUImanager.Init(text,sprite);
    }

    public void DisableTextUIManager()
    {
        _textUImanager.Disable();
    }

    public void ShowInfoUIManager(string text)
    {
        _uiManager.InfoDeploy(text);
    }
}
