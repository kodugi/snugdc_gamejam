using UnityEngine;
using UnityEngine.UI;

public class ButtonContainer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WordData[][] test = new WordData[3][];
        for (int i = 0; i < 3; i++)
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
        for(int row = 0 ; row<wordLists.Length ; row++)
        {
            WordData[] wordList = wordLists[row];
            buttonlists[row] = new ButtonHandler[wordList.Length];
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
            for(int col = 0 ; col<wordList.Length ; col++)
            {
                Debug.Log(col);
                WordData wordData = wordList[col];
                GameObject goButton = Instantiate(button, go.transform);
                ButtonHandler buttonScript = goButton.GetComponent<ButtonHandler>();
                buttonlists[row][col]=buttonScript;
                buttonScript.Init(wordData,row,col);
            }
        }
    }

    public void DisableButton(int row, int col)
    {
        buttonlists[row][col].Disablebutton();
    }

    public void DisableColumn(int col)
    {
        for (int row = 0; row < buttonlists.Length; row++)
        {
            buttonlists[row][col].Disablebutton();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
