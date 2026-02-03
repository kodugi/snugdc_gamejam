using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
public class UIManager: MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI TMP_RemainingChoices;
    [SerializeField] private TurnEndButton _turnEndButton;
    [SerializeField] private TMPro.TextMeshProUGUI TMP_Score;
    [SerializeField] private CanvasGroup _PlayField;
    [SerializeField] private CanvasGroup _ItemField;
    [SerializeField] private CanvasGroup _MainField;
    [SerializeField] private ItemList playerInventory;
    [SerializeField] private ItemList enemyInventory;
    [SerializeField] private TextUIManager textUIManager;
    [SerializeField] private InfoBoxManager infoBoxManager;
    [SerializeField] private PlayerSpriteController playerSpriteController;
    [SerializeField] private PlayerSpriteController enemySpriteController;
    [SerializeField] private EndingController endingController;
    
    public void UpdateRemainingChoices(int remainingChoices)
    {
        TMP_RemainingChoices.text = remainingChoices + "/2";
    }

    public void UpdateSkipButton(int remainingChoices)
    {
        if (remainingChoices >= 2)
        {
            DisableSkip();
        }
        else
        {
            EnableSkip();
        }
    }

    public void UpdateScore(int playerScore, int enemyScore)
    {
        TMP_Score.text = $"{playerScore}:{enemyScore}";
    }

    public void EnableSkip()
    {
        _turnEndButton.Enable();
    }

    public void DisableSkip()
    {
        _turnEndButton.Disable();
    }

    public void DisableAll()
    {
        _PlayField.blocksRaycasts = false;
        _ItemField.blocksRaycasts = false;
    }
    public void DisDisableAll()
    {
        _PlayField.blocksRaycasts = true;
        _ItemField.blocksRaycasts = true;
    }

    public void ItemUpdate(DefaultDictionary<ItemType, int> playerinventory,DefaultDictionary<ItemType, int> enemyinventory)
    {
        Debug.Log("Item update called");
        playerInventory.Render(playerinventory,true);
        enemyInventory.Render(enemyinventory,false);
    }

    public void InfoDeploy(string text,float time)
    {
        infoBoxManager.CloneAndDeploy(text,time);
    }
    public void PlayerWinRender()
    {
        playerSpriteController.Win();
        enemySpriteController.Lose();
    }

    public void EnemyWinRender()
    {
        playerSpriteController.Lose();
        enemySpriteController.Win();
    }

    public void PlayersSpriteReset()
    {
        playerSpriteController.reset();
        enemySpriteController.reset();
    }

    public void SetEndUI(bool iswin)
    {
        endingController.gameObject.SetActive(true);
        endingController.setEnd(iswin);
    }
}