using System.Net.Mime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemItem : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    private ItemType _itemType;
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
                break;
            case ItemType.Americano:
                _image.sprite = sprites[1];
                break;
            case ItemType.Beer:
                _image.sprite = sprites[2];
                break;
            case ItemType.Gloves:
                _image.sprite = sprites[3];
                break;
            case ItemType.Transceiver:
                _image.sprite = sprites[4];
                break;
            case ItemType.MagnifyingGlass:
                _image.sprite = sprites[5];
                break;
        }
    }

    private void OnUse()
    {
        _button.enabled = false;
        GameManager.Instance.PlayItem(_itemType);
        Destroy(this.gameObject);
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
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
