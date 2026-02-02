using System.Net.Mime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemItem : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private ItemType _itemType;
    private string _explanation;
    private Button _button;
    [SerializeField] Image _image;
    [SerializeField] private Sprite[] sprites;
    public void Init(ItemType itemType,bool isPlayers)
    {
        _itemType = itemType;
        if (isPlayers)
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnUse);
        }

        switch (itemType)
        {
            case ItemType.AncientDocument:
                _image.sprite = sprites[0];
                _explanation = "‘조사’의 위치(열)를 2곳 알려줍니다.";
                break;
            case ItemType.Americano:
                _image.sprite = sprites[1];
                _explanation = "플레이어가 단어를 한 번 더 고를 수 있도록 합니다.";
                break;
            case ItemType.Beer:
                _image.sprite = sprites[2];
                _explanation = "틀릴 때까지 단어를 선택합니다. 틀릴 시, 패배합니다.";
                break;
            case ItemType.Gloves:
                _image.sprite = sprites[3];
                _explanation = "상대방의 무작위 아이템을 훔쳐옵니다.";
                break;
            case ItemType.Transceiver:
                _image.sprite = sprites[4];
                _explanation = "랜덤한 위치의 정답을 한 개 알려줍니다.";
                break;
            case ItemType.MagnifyingGlass:
                _image.sprite = sprites[5];
                _explanation = "돋보기는 선택한 칸의 위치가 정답인지, 오답인지 알려줍니다.";
                break;
        }
    }

    private void OnUse()
    {
        _button.enabled = false;
        GameManager.Instance.PlayItem(_itemType);
        GameManager.Instance.DisableTextUIManager();
        Destroy(this.gameObject);
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("hovered");
        GameManager.Instance.ShowTextUIManager(_explanation,_image.sprite);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("hover exited");
        GameManager.Instance.DisableTextUIManager();
    }
    private void OnStolen()
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
