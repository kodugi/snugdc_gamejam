using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] Canvas playCanvas;

    [SerializeField] private Canvas titleCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Button StartButton;
    [SerializeField] Button HomeButton;
    [SerializeField] Button ExitButton;
    [SerializeField] Button TutorialButton;
    private SoundManager _soundManager;
    
    private void StartGame()
    {
        PlayButtonSound();
        _soundManager.PlayMusic(AudioType.IngameScreenMusic);
        
        titleCanvas.transform.gameObject.SetActive(false);
        playCanvas.transform.gameObject.SetActive(true);
        GameManager.Instance.StartGame();
    }

    private void EndGame()
    {
        PlayButtonSound();
        _soundManager.PlayMusic(AudioType.TitleScreenMusic);
        titleCanvas.transform.gameObject.SetActive(true);
        playCanvas.transform.gameObject.SetActive(false);
    }

    private void OpenTutorial()
    {
        PlayButtonSound();
        _soundManager.PlayMusic(AudioType.TutorialMusic);
        titleCanvas.transform.gameObject.SetActive(false);
        playCanvas.transform.gameObject.SetActive(false);
        SceneManager.LoadScene("TutorialScene", LoadSceneMode.Additive);
    }

    private void PlayButtonSound()
    {
        _soundManager.PlaySound(AudioType.ButtonClick);
    }

    private void CloseTutorial(Scene scene)
    {
        if (scene.name == "TutorialScene")
        {
            PlayButtonSound();
            _soundManager.PlayMusic(AudioType.TitleScreenMusic);
            titleCanvas.transform.gameObject.SetActive(true);
            playCanvas.transform.gameObject.SetActive(false);
        }   
    }

    

    public void Awake()
    {
        StartButton.onClick.AddListener(StartGame);
        HomeButton.onClick.AddListener(EndGame);
        ExitButton.onClick.AddListener(Application.Quit);
        TutorialButton.onClick.AddListener(OpenTutorial);
        SceneManager.sceneUnloaded+= CloseTutorial;
    }
    
    public void Start()
    {
        _soundManager = GetComponent<SoundManager>();
    }
}
