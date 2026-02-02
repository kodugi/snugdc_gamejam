using UnityEngine;

public class Americano : IItem
{
    public void Use(GameManager gameManager)
    {
        gameManager.AddRemainingChoices(2);
    }
}
