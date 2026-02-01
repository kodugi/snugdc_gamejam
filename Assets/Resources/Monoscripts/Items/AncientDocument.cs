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
                    // Highlight the column
                    gameManager.ButtonContainer.HighLightColumn(colToReveal);
                    break;
                }
            }
        }
    }
}
