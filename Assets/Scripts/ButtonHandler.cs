using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] Image highlightImage;
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    private Collider2D collider2D;
    public event Action<int, int,bool> OnButtonClicked;
    
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
        OnClick();
    }
    void OnClick()
    {
        Debug.Log("clicked");
        GameManager.Instance.ProcessWordChoice(row,col);
        OnButtonClicked?.Invoke(row,col,isCorrect);
    }

    public void HighLight()
    {
        transform.localScale = Vector3.one * 1.2f;
        highlightImage.enabled = true;
    }
    public void UnHighLight()
    {
        transform.localScale = Vector3.one;
        highlightImage.enabled = false;
    }
    public void Disablebutton()
    {
     collider2D.enabled = false;
    }
}
