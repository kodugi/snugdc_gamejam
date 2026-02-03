using System.Collections.Generic;

public class Player
{
    public int playerId;
    public DefaultDictionary<ItemType, int> inventory = new DefaultDictionary<ItemType, int>();
    public int GetTotalItems()
    {
        int total = 0;
        foreach (var itemCount in inventory.Values)
        {
            total += itemCount;
        }
        return total;
    }
}