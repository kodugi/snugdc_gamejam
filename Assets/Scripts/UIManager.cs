using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
public class UIManager: MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI TMP_RemainingChoices;

    public void UpdateRemainingChoices(int remainingChoices)
    {
        TMP_RemainingChoices.text = remainingChoices + "/2";
    }
}