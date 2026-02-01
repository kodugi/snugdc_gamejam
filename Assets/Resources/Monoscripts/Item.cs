using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    void Use(GameManager gameManager);
}

public enum ItemType
{
    Transceiver,
    MagnifyingGlass,
    Americano,
    AncientDocument,
    Gloves,
    Beer
}

public class ItemData
{
    public string itemName;
    public ItemType type;
}
