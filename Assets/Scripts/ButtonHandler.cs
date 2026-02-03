using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] Image highlightImage;
    [SerializeField] Image IncorrectHider;
    [SerializeField] Image CorrectHider;
    [SerializeField] Image UnHighlightImage;
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private Button _button;
    
    private string text = "";
    private WordType wordType;
    private bool isCorrect = false;

    private int row, col;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(WordData wordData,int row,int col)
    {
        UnHighLight();
        IncorrectHider.enabled = false;
        CorrectHider.enabled = false;
        UnHighlightImage.enabled = false;
        _button.onClick.AddListener(OnClick);
        text=wordData.word;
        SetText(text);
        wordType=wordData.type;
        this.isCorrect=wordData.isCorrect;
        this.row=row;
        this.col=col;
    }
    private void SetText(string word)
    {
        _textMeshPro.text = word;
        // 길이에 따른 크기 조정
        if(word.Length >= 4)
        {
            _textMeshPro.fontSize = 36;
        }
        else
        {
            _textMeshPro.fontSize = 54;
        }
    }
    void OnClick()
    {
        GameManager.Instance.ProcessWordChoice(row,col);
    }

    public void HighLight()
    {
        if (!_button.interactable) return;
        highlightImage.enabled = true;
        UnHighlightImage.enabled = false;
    }
    public void UnHighLight()
    {
        if(_button.interactable)
        transform.localScale = Vector3.one;
        highlightImage.enabled = false;
        if (!_button.interactable && isCorrect) return;
        UnHighlightImage.enabled = true;
    }

    public void UnUnHighLight()
    {
        if (!_button.interactable) return;
        UnHighlightImage.enabled = false;
    }
    public void Disablebutton()
    {
        UnHighlightImage.enabled = true;
        if (isCorrect)
        {
            Disablecorrect();
        }
        else
        {
            DisableIncorrect();
        }

        _button.interactable = false;
    }

    private void DisableIncorrect()
    {
        transform.localScale = Vector3.one * 0.8f;
        IncorrectHider.enabled = true;
    }

    private void Disablecorrect()
    {
        CorrectHider.enabled = true;
    }
}
