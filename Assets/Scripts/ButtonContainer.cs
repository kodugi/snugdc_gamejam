using UnityEngine;
using UnityEngine.UI;

public class ButtonContainer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WordData[][] test = new WordData[4][];
        for (int i = 0; i < 4; i++)
        {
            test[i]=new WordData[3];
            for (int j = 0; j < 3; j++)
            {
                WordData testdata=new WordData();
                testdata.type = WordType.Verb;
                testdata.word = "123";
                testdata.isCorrect=false;
                test[i][j]=testdata;
            }
        }
        Init(test);
        HighLightColumn(0);
    }

    [SerializeField] private float verticalpaddingheight;
    [SerializeField] private float horizontialpaddingwidth;
    [SerializeField] private GameObject button;
    private ButtonHandler[][] buttonlists;
    public void Init(WordData[][] wordLists)
    {
        buttonlists = new ButtonHandler[wordLists.Length][];
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        for(int col = 0 ; col<wordLists.Length ; col++)
        {
            WordData[] wordList = wordLists[col];
            buttonlists[col] = new ButtonHandler[wordList.Length];
            GameObject go = Instantiate(new GameObject(),transform);
            go.name = "ButtonList";
            go.transform.SetParent(transform,false);
            VerticalLayoutGroup verticalLayoutGroup = go.AddComponent<VerticalLayoutGroup>();
            LayoutElement layoutElement = go.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = transform.GetComponent<RectTransform>().sizeDelta.y;
            layoutElement.preferredWidth = transform.GetComponent<RectTransform>().sizeDelta.x/wordLists.Length;
            
            verticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            verticalLayoutGroup.childControlWidth = false;
            verticalLayoutGroup.childControlHeight = false;
            verticalLayoutGroup.spacing = verticalpaddingheight;
            for(int row = 0 ; row<wordList.Length ; row++)
            {
                Debug.Log(row);
                WordData wordData = wordList[row];
                GameObject goButton = Instantiate(button, go.transform);
                ButtonHandler buttonScript = goButton.GetComponent<ButtonHandler>();
                buttonScript.OnButtonClicked += HandleButtonClicked;
                buttonlists[col][row]=buttonScript;
                buttonScript.Init(wordData,row,col);
            }
        }
    }

    public void HandleButtonClicked(int row, int col,bool isCorrect)
    {
        if (isCorrect)
        {
            DisableColumn(col);
        }
        else
        {
            DisableButton(row,col);
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
        for (int row = 0; row < buttonlists[col].Length; row++)
        {
            buttonlists[col][row].HighLight();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
