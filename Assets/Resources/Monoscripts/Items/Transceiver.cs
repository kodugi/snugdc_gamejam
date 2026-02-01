using System.Collections.Generic;
using UnityEngine;

public class Transceiver : IItem
{
    public void Use(GameManager gameManager)
    {
        // Logic from PlayTransceiver
        List<(int, int)> correctLocations = new List<(int, int)>(); // (row, col)
        var currentSentenceData = gameManager.GetCurrentSentenceData();
        for (int col = 0; col < currentSentenceData.sentences.Count; col++)
        {
            for (int row = 0; row < currentSentenceData.sentences[col].Count; row++)
            {
                WordData wordData = currentSentenceData.sentences[col][row];
                if (wordData.type == WordType.Conjunction && wordData.isCorrect)
                {
                    correctLocations.Add((row, col));
                    break;
                }
            }
        }

        if (correctLocations.Count > 0)
        {
            int randomIndex = Random.Range(0, correctLocations.Count);
            // Highlight the word at correctLocations[randomIndex]
            gameManager.ButtonContainer.HighlightButton(correctLocations[randomIndex].Item2, correctLocations[randomIndex].Item1);
        }
    }
}
