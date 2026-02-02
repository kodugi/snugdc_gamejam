using UnityEngine;

public class ItemList : MonoBehaviour
{
    [SerializeField] GameObject ItemPrefab;
    public void Render(DefaultDictionary<ItemType, int> inventory,bool isplayer)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        foreach(var item in inventory)
        {
            if (item.Key == ItemType.None) continue;
            if (item.Value == 0) continue;
            for(int i = 0; i < item.Value; i++)
            {
                GameObject newItem = Instantiate(ItemPrefab,this.transform);
                ItemItem itemScript = newItem.GetComponent<ItemItem>();
                itemScript.Init(item.Key,isplayer);
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
