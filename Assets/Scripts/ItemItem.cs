using System.Net.Mime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemItem : MonoBehaviour,IPointerClickHandler
{
    private ItemType _itemType;
    [SerializeField] Image _image;
    public void Init(ItemType itemType)
    {
        _itemType = itemType;
        Render();
    }

    public void OnPointerClick(PointerEventData data)
    {
        OnUse();
    }
    private void Render()
    {
        switch (_itemType)
        {
            case ItemType.Americano:
                _image.sprite = Resources.Load<Sprite>("Sprites/Americano") as Sprite;
                break;
            case ItemType.Beer:
                _image.sprite = Resources.Load<Sprite>("Sprites/Beer") as Sprite;
                break;
            case ItemType.Gloves:
                _image.sprite = Resources.Load<Sprite>("Sprites/Gloves") as Sprite;
                break;
            case ItemType.AncientDocument:
                _image.sprite = Resources.Load<Sprite>("Sprites/AncientDocument") as Sprite;
                break;
            case ItemType.MagnifyingGlass:
                _image.sprite = Resources.Load<Sprite>("Sprites/MagnifyingGlass") as Sprite;
                break;
            case ItemType.Transceiver:
                _image.sprite = Resources.Load<Sprite>("Sprites/Transceiver") as Sprite;
                break;
        }


    }

    private void OnUse()
    {
        GameManager.Instance.PlayItem(_itemType);
        Destroy(this.gameObject);
        
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
