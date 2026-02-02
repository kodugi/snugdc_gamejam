using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TMPro.TextMeshProUGUI tutorialText;
    [SerializeField] private Image tutorialImage;
    private int currentStep = 0;

    private TutorialData tutorialData;

    private void Start()
    {
        TextAsset tutorialJson = Resources.Load<TextAsset>("TutorialData/TutorialDialogue");
        tutorialData = JsonUtility.FromJson<TutorialData>(tutorialJson.text);

        nextButton.onClick.AddListener(OnNextButtonClicked);
        prevButton.onClick.AddListener(OnPrevButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        ShowStep(currentStep);
    }

    private void ShowStep(int stepIndex)
    {
    prevButton.interactable = !stepIndex.Equals(0);
    nextButton.interactable = !stepIndex.Equals(tutorialData.steps.Length-1);
        
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
    private void OnPrevButtonClicked()
    {
        currentStep--;
        ShowStep(currentStep);
    }

    private void OnQuitButtonClicked()
    {
        SceneManager.LoadScene("UIScene");
    }
}
