using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingController : MonoBehaviour
{
    [SerializeField] private Sprite winPortrait;
    [SerializeField] private Sprite losePortrait;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;
    private Image image;

    private float t;

    [SerializeField] private float fadeinTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void setEnd(bool iswin)
    {
        image = GetComponent<Image>();
        image.sprite = iswin ? winPortrait : losePortrait;
        text.text = iswin ? "승리!" : "패배!";
        button = GetComponentInChildren<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(restart);
        image.color = Color.black;
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
        if (t < fadeinTime)
        {
            t += Time.deltaTime;
            image.color = Color.Lerp(Color.black, Color.white, t / fadeinTime);
        }
    }
}
