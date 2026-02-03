using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonContainer : MonoBehaviour
{ 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    [SerializeField] private float verticalpaddingheight;
    [SerializeField] private float horizontialpaddingwidth;
    [SerializeField] private GameObject button;
    private ButtonHandler[][] buttonlists;
    public void Init(SentenceData sentenceData)
    {
        List<List<WordData>> wordLists=sentenceData.sentences;
        
        buttonlists = new ButtonHandler[wordLists.Count][];
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        for(int col = 0 ; col<wordLists.Count ; col++)
        {
            List<WordData> wordList = wordLists[col];
            buttonlists[col] = new ButtonHandler[wordList.Count];
            GameObject go = Instantiate(new GameObject(),transform);
            go.name = "ButtonList";
            go.transform.SetParent(transform,false);
            VerticalLayoutGroup verticalLayoutGroup = go.AddComponent<VerticalLayoutGroup>();
            LayoutElement layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = transform.GetComponent<RectTransform>().sizeDelta.y;
            layoutElement.preferredWidth = 75;
            
            verticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            verticalLayoutGroup.childControlWidth = false;
            verticalLayoutGroup.childControlHeight = false;
            verticalLayoutGroup.spacing = verticalpaddingheight;
            for(int row = 0 ; row<wordList.Count ; row++)
            {
                WordData wordData = wordList[row];
                GameObject goButton = Instantiate(button, go.transform);
                ButtonHandler buttonScript = goButton.GetComponent<ButtonHandler>();
                buttonlists[col][row]=buttonScript;
                buttonScript.Init(wordData,row,col);
            }
        }
    }
    public void DisableButton(int row, int col)
    {
        buttonlists[col][row].Disablebutton();
    }

    public void DisableColumn(int col)
    {
        for (int row = 0; row < buttonlists[col].Length; row++)
        {
            buttonlists[col][row].Disablebutton();
        }
    }

    public void UnHighLightAll()
    {
        foreach(ButtonHandler[] buttonlist in buttonlists)
        {
            foreach (ButtonHandler buttonHandler in buttonlist)
            {
                buttonHandler.UnHighLight();
            }
        }
    }
    public void HighlightButton(int row, int col)
    {
        buttonlists[col][row].HighLight();
    }

    public void HighLightColumn(int col)
    {
        if (col >= buttonlists.Length) return;
        for (int row = 0; row < buttonlists[col].Length; row++)
        {
            buttonlists[col][row].HighLight();
        }
    }public void UnUnHighLightColumn(int col)
    {
        if (col >= buttonlists.Length) return;
        for (int row = 0; row < buttonlists[col].Length; row++)
        {
            buttonlists[col][row].UnUnHighLight();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
