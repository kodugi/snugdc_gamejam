using System.Collections.Generic;

public class Player
{
    public int playerId;
    public DefaultDictionary<ItemType, int> inventory = new DefaultDictionary<ItemType, int>();
}