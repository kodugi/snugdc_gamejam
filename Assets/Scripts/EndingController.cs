using UnityEngine;
using UnityEngine.UI;

public class EndingController : MonoBehaviour
{
    [SerializeField] private Sprite winPortrait;
    [SerializeField] private Sprite losePortrait;
    [SerializeField] private Button button;
    private Image image;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void setEnd(bool iswin)
    {
        image = GetComponent<Image>();
        image.sprite = iswin ? winPortrait : losePortrait;
        button = GetComponentInChildren<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(restart);
    }

    private void restart()
    {
        GameManager.Instance.StartGame();
        gameObject.SetActive(false);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
