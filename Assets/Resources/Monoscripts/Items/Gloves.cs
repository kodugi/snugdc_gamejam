using System.Collections.Generic;
using UnityEngine;

public class Gloves : IItem
{
    public void Use(GameManager gameManager)
    {
        // Logic from PlayGloves
        Player opponent = gameManager.GetOpponent();
        List<ItemData> opponentItems = new List<ItemData>(opponent.inventory.Keys);
        if (opponentItems.Count > 0)
        {
            ItemData stolenItem = opponentItems[Random.Range(0, opponentItems.Count)];
            while (opponent.inventory[stolenItem] <= 0) // 아이템 개수가 0인 경우 다시 선택
            {
                stolenItem = opponentItems[Random.Range(0, opponentItems.Count)];
            }

            opponent.inventory[stolenItem]--;
            gameManager.GetCurrentPlayer().inventory[stolenItem]++;
        }
    }
}
