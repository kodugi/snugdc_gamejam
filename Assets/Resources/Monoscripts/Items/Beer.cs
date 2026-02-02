using UnityEngine;

public class Beer : IItem
{
    public void Use(GameManager gameManager)
    {
        gameManager.SetDrankBeer(true);
    }
}
