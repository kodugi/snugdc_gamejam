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
    private SoundManager _soundManager;

    private void Start()
    {
        TextAsset tutorialJson = Resources.Load<TextAsset>("TutorialData/TutorialDialogue");
        tutorialData = JsonUtility.FromJson<TutorialData>(tutorialJson.text);
        _soundManager = GetComponent<SoundManager>();

        nextButton.onClick.AddListener(OnNextButtonClicked);
        prevButton.onClick.AddListener(OnPrevButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        ShowStep(currentStep);
    }

    private void ShowStep(int stepIndex)
    {
    prevButton.interactable = !stepIndex.Equals(0);
        
        if (stepIndex < tutorialData.steps.Length)
        {
            tutorialText.text = tutorialData.steps[stepIndex].text;
            string imagePath = tutorialData.steps[stepIndex].imagePath;
            Texture2D tutorialTexture = Resources.Load<Texture2D>(imagePath);

            if (tutorialTexture != null)
            {
                Sprite tutorialSprite = Sprite.Create(tutorialTexture, new Rect(0, 0, tutorialTexture.width, tutorialTexture.height), new Vector2(0.5f, 0.5f));
                tutorialImage.sprite = tutorialSprite;
            }
            else
            {
                Debug.LogError($"Tutorial Image not found at path: {imagePath}");
            }
            if(stepIndex == tutorialData.steps.Length - 1)
            {
                nextButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "시작하기";
            }
            else
            {
                nextButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "다음으로";
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
        _soundManager.GetComponent<AudioSource>().PlayOneShot(_soundManager.ButtonClickSound);
        if (currentStep >= tutorialData.steps.Length - 1)
        {
            SceneManager.UnloadSceneAsync("TutorialScene");
            return;
        }
        currentStep++;
        ShowStep(currentStep);
    }
    private void OnPrevButtonClicked()
    {
        _soundManager.GetComponent<AudioSource>().PlayOneShot(_soundManager.ButtonClickSound);
        currentStep--;
        ShowStep(currentStep);
    }

    private void OnQuitButtonClicked()
    {
        _soundManager.GetComponent<AudioSource>().PlayOneShot(_soundManager.ButtonClickSound);
        SceneManager.UnloadSceneAsync("TutorialScene");
    }
}
