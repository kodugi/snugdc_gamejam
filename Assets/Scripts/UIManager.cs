using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
public class UIManager: MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI TMP_RemainingChoices;
    [SerializeField] private TurnEndButton _turnEndButton;
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
}