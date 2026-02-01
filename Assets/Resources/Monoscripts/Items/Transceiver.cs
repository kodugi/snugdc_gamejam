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
            if(gameManager.GetCurrentPlayer().playerId == 0) // 플레이어
            {
                Debug.Log($"The word at column {correctLocations[randomIndex].Item2 + 1}, row {correctLocations[randomIndex].Item1 + 1} is correct!");
            }
            else // 적
            {
                for(int curRow = 0; curRow < currentSentenceData.sentences[correctLocations[randomIndex].Item2].Count; curRow++)
                {
                    if(correctLocations[randomIndex].Item1 == curRow)
                    {
                        continue;
                    }
                    ((Enemy)gameManager.GetCurrentPlayer()).RemoveAt(new Position(curRow, correctLocations[randomIndex].Item2)); // 정답이 아닌 경우 후보군에서 제외
                }
            }
        }
    }
}
