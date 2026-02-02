using UnityEngine;

public class ItemList : MonoBehaviour
{
    [SerializeField] GameObject ItemPrefab;
    [SerializeField] Sprite[] sprites;
    public void Render(DefaultDictionary<ItemType, int> inventory)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        foreach(var item in inventory)
        {
            if (item.Value == 0) continue;
            for(int i = 0; i < item.Value; i++)
            {
                GameObject newItem = Instantiate(ItemPrefab);
                ItemItem itemScript = newItem.GetComponent<ItemItem>();
                itemScript.Init(item.Key);
            }
        }
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
