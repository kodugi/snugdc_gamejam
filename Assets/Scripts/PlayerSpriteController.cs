using UnityEngine;
using UnityEngine.UI;

public class PlayerSpriteController : MonoBehaviour
{
    [SerializeField] private Sprite idle;
    [SerializeField] private Sprite lose;
    [SerializeField] private Sprite win;

    private Image _arrowimage;
    /*public Image ArrowImage
    {
        get
        {
            if (_arrowimage == null)
            {
                _arrowimage = transform.Find("Arrow").GetComponent<Image>();
            }
            return _arrowimage;
        }
    }*/

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
        /*ArrowImage.enabled = false;*/
    }

    public void MyTurn()
    {
        /*ArrowImage.enabled = true;*/
    }

    public void NotMyTurn()
    {
        /*ArrowImage.enabled = false;*/
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
