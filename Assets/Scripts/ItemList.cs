using UnityEngine;

public class ItemList : MonoBehaviour
{
    [SerializeField] GameObject ItemPrefab;
    public void AddItem(ItemType item)
    {
        GameObject newItem = Instantiate(ItemPrefab);
        ItemItem itemScript = newItem.GetComponent<ItemItem>();
        itemScript.Init(item);
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
