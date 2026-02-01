using System.Collections.Generic;

public class Player
{
    public int playerId;
    public DefaultDictionary<ItemData, int> inventory = new DefaultDictionary<ItemData, int>();
}