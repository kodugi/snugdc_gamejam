using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] Image highlightImage;
    [SerializeField] Image IncorrectHider;
    [SerializeField] Image CorrectHider;
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private Collider2D collider2D;
    
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
        collider2D.enabled = true;
        highlightImage.transform.localScale=Vector3.one*1.3f;
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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!collider2D.enabled)
            return;
        OnClick();
    }
    void OnClick()
    {
        Debug.Log("clicked "+row+", "+col);
        GameManager.Instance.ProcessWordChoice(row,col);
    }

    public void HighLight()
    {
        if (!collider2D.enabled) return;
        highlightImage.enabled = true;
    }
    public void UnHighLight()
    {
        if(collider2D.enabled)
        transform.localScale = Vector3.one;
        highlightImage.enabled = false;
    }
    public void Disablebutton()
    {
        if (isCorrect)
        {
            Disablecorrect();
        }
        else
        {
            DisableIncorrect();
        }
        collider2D.enabled = false;
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
