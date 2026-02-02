using System.Net.Mime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemItem : MonoBehaviour
{
    private ItemType _itemType;
    private Button _button;
    [SerializeField] Image _image;
    public void Init(ItemType itemType)
    {
        _itemType = itemType;
        _button = GetComponent<Button>();
        switch (itemType)
        {
            case ItemType.AncientDocument:
                _image.sprite = Resources.Load<Sprite>("Sprites/AncientDocument");
                break;
            case ItemType.Americano:
                _image.sprite = Resources.Load<Sprite>("Sprites/Americano");
                break;
            case ItemType.Beer:
                _image.sprite = Resources.Load<Sprite>("Sprites/Beer");
                break;
            case ItemType.Gloves:
                _image.sprite = Resources.Load<Sprite>("Sprites/Gloves");
                break;
            case ItemType.Transceiver:
                _image.sprite = Resources.Load<Sprite>("Sprites/Transceiver");
                break;
            case ItemType.MagnifyingGlass:
                _image.sprite = Resources.Load<Sprite>("Sprites/MagnifyingGlass");
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
