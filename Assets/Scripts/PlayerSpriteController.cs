using UnityEngine;
using UnityEngine.UI;

public class PlayerSpriteController : MonoBehaviour
{
    [SerializeField] private Sprite idle;
    [SerializeField] private Sprite lose;
    [SerializeField] private Sprite win;

    private Image _image;
    public Image Image
    {
        get
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }
            return _image;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _image = GetComponent<Image>();
        Image.sprite = idle;
    }

    public void Win()
    {
        Image.sprite = win;
    }

    public void Lose()
    {
        Image.sprite = lose;
    }

    public void reset()
    {
        Image.sprite = idle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
