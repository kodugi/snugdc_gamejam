using System.Collections.Generic;

public class Enemy: Player
{
    public int playerId = 1;

    public Position getNextChoice(HashSet<int> correctColumns, int currentColumn, SentenceData sentenceData)
    {
        return new Position(0, currentColumn);
    }
}