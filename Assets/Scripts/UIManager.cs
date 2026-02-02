using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
public class UIManager: MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI TMP_RemainingChoices;
    [SerializeField] private TurnEndButton _turnEndButton;
    [SerializeField] private CanvasGroup _PlayField;
    [SerializeField] private CanvasGroup _ItemField;
    [SerializeField] private CanvasGroup _MainField;
    [SerializeField] private ItemList playerInventory;
    [SerializeField] private ItemList enemyInventory;
    public void UpdateRemainingChoices(int remainingChoices)
    {
        TMP_RemainingChoices.text = remainingChoices + "/2";
    }

    public void UpdateSkipButton(int remainingChoices)
    {
        if (remainingChoices == 2)
        {
            DisableSkip();
        }
        else
        {
            EnableSkip();
        }
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
        _MainField.blocksRaycasts = false;
    }
    public void DisDisableAll()
    {
        _MainField.blocksRaycasts = true;
    }

    public void ItemUpdate(DefaultDictionary<ItemType, int> playerinventory,DefaultDictionary<ItemType, int> enemyinventory)
    {
        playerInventory.Render(playerinventory);
        enemyInventory.Render(enemyinventory);
    }
}