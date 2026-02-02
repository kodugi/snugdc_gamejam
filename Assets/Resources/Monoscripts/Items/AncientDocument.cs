using System.Collections.Generic;
using UnityEngine;

public class AncientDocument : IItem
{
    public void Use(GameManager gameManager)
    {
        // Logic from PlayAncientDocument
        List<int> conjunctionColumns = new List<int>();
        var currentSentenceData = gameManager.GetCurrentSentenceData();
        for (int col = 0; col < currentSentenceData.sentences.Count; col++)
        {
            for (int row = 0; row < currentSentenceData.sentences[col].Count; row++)
            {
                WordData wordData = currentSentenceData.sentences[col][row];
                if (wordData.type == WordType.Conjunction && wordData.isCorrect)
                {
                    conjunctionColumns.Add(col);
                    break;
                }
            }
        }

        int revealCount = Mathf.Min(2, conjunctionColumns.Count);
        string output = "";
        for (int i = 0; i < revealCount; i++)
        {
            int randomIndex = Random.Range(0, conjunctionColumns.Count);
            int colToReveal = conjunctionColumns[randomIndex];
            conjunctionColumns.RemoveAt(randomIndex);

            for (int row = 0; row < currentSentenceData.sentences[colToReveal].Count; row++)
            {
                WordData wordData = currentSentenceData.sentences[colToReveal][row];
                if (wordData.type == WordType.Conjunction && wordData.isCorrect)
                {
                    if(gameManager.GetCurrentPlayer().playerId == 0) // 플레이어
                    {
                        output += $"{colToReveal + 1}번째 줄에 조사가 있습니다.\n";
                    }
                    else // 적
                    {
                        for(int curRow = 0; curRow < currentSentenceData.sentences[colToReveal].Count; curRow++)
                        {
                            if(currentSentenceData.sentences[colToReveal][curRow].type == WordType.Conjunction)
                            {
                                continue;
                            }
                            ((Enemy)gameManager.GetCurrentPlayer()).RemoveAt(new Position(curRow, colToReveal)); // 조사가 아닌 경우 후보군에서 제외
                        }
                    }
                    break;
                }
            }
        }

        if (output == "") output = "남은 조사가 없습니다.";
        if(gameManager.GetCurrentPlayer().playerId == 0) 
        gameManager.ShowInfoUIManager(output,2f);
    }
}
