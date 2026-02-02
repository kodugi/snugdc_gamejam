using System;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] Canvas playCanvas;

    [SerializeField] private Canvas titleCanvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Button StartButton;
    [SerializeField] Button HomeButton;
    [SerializeField] Button ExitButton;
    
    public void StartGame()
    {
        titleCanvas.enabled = false;
        playCanvas.enabled = true;
        GameManager.Instance.StartGame();
    }

    public void EndGame()
    {
        titleCanvas.enabled = true;
        playCanvas.enabled = false;
    }

    public void Awake()
    {
        StartButton.onClick.AddListener(StartGame);
        HomeButton.onClick.AddListener(EndGame);
        ExitButton.onClick.AddListener(Application.Quit);
    }
}
