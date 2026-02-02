using System.Collections.Generic;
using UnityEngine;

public class MagnifyingGlass : IItem
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
                if (wordData.isCorrect)
                {
                    correctLocations.Add((row, col));
                    break;
                }
            }
        }
        if (correctLocations.Count > 0)
        {
            int randomIndex=-1;
            for (int i = 0; i < correctLocations.Count; i++)
            {
                if (correctLocations[i].Item2 == GameManager.Instance.GetCurrentColumn())
                {
                    randomIndex = i;
                    break;
                }
            }

            if (randomIndex == -1)
            {
                gameManager.ShowInfoUIManager($"안알려줄건데.");
                return;

            }
            // Highlight the word at correctLocations[randomIndex]
            if(gameManager.GetCurrentPlayer().playerId == 0) // 플레이어
            {
                gameManager.ShowInfoUIManager($"현재 줄의 {correctLocations[randomIndex].Item1 + 1}번째 단어는 옳습니다.",2f);
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
