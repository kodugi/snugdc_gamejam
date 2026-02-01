using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    private Collider2D collider2D;
    
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
        onClick();
    }
    public void onClick()
    {
        Debug.Log("clicked");
    }

    public void Disablebutton()
    {
     collider2D.enabled = false;
    }
}
