using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private TMPro.TextMeshProUGUI tutorialText;
    [SerializeField] private Image tutorialImage;
    private int currentStep = 0;

    private TutorialData tutorialData;

    private void Start()
    {
        TextAsset tutorialJson = Resources.Load<TextAsset>("TutorialData/TutorialDialogue");
        tutorialData = JsonUtility.FromJson<TutorialData>(tutorialJson.text);

        nextButton.onClick.AddListener(OnNextButtonClicked);

        ShowStep(currentStep);
    }

    private void ShowStep(int stepIndex)
    {
        if (stepIndex < tutorialData.steps.Length)
        {
            tutorialText.text = tutorialData.steps[stepIndex].text;
            Sprite tutorialSprite = Resources.Load<Sprite>(tutorialData.steps[stepIndex].imagePath);
            if (tutorialSprite != null)
            {
                tutorialImage.sprite = tutorialSprite;
            }
            else
            {
                Debug.LogError($"Tutorial Image not found at path: {tutorialData.steps[stepIndex].imagePath}");
            }
        }
        else
        {
            // End of tutorial, maybe hide the tutorial UI
            gameObject.SetActive(false);
        }
    }

    private void OnNextButtonClicked()
    {
        currentStep++;
        ShowStep(currentStep);
    }
}
