using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Enemy: Player
{
    private GameManager _gameManager;
    private TurnManager _turnManager;
    private List<List<bool>> _availableWords;
    private bool _isBeerEffectActive = false;

    public async Task PlayTurnAsync()
    {
        _isBeerEffectActive = false; // 턴 시작 시 초기화
        int i = 0;
        while (true)
        {
            await Task.Delay(1000+Random.Range(0,500));
            i++;
            if (i == 10) break;
            ItemType useItem = UseItem();
            if (useItem == ItemType.None)
                break;
            
            if (useItem == ItemType.Beer)
            {
                _isBeerEffectActive = true;
            }

            Debug.Log("Enemy uses item: " + useItem);
            _gameManager.PlayItem(useItem);
        }

        await Task.Delay(1000+Random.Range(0,500));
        Position first = getNextChoice();
        Debug.Log("Enemy first 선택: Column " + first.col + ", Row " + first.row);
        _turnManager.ProcessWordChoice(first.row, first.col);
        while(_gameManager.GetRemainingChoices()>0 && _gameManager.GetCurrentPlayer().playerId==1)
        {
            await Task.Delay(1000+Random.Range(0,500));
            Position second = getNextChoice();
            if (second.row != -1)
            {
                _turnManager.ProcessWordChoice(second.row, second.col);
            }
            else break;
        }
        if( _gameManager.GetCurrentPlayer().playerId==1)
        {
            _turnManager.TurnEnd();
        }
        _isBeerEffectActive = false; // 턴 종료 시 초기화
    }

    public Position getNextChoice()
    {
        int currentColumn = _gameManager.GetCurrentColumn();
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < _availableWords[currentColumn].Count; i++)
        {
            if (_availableWords[currentColumn][i])
            {
                availableIndices.Add(i);
            }
        }

        int correctTilesLeftTotal = 0;
        for (int c = _gameManager.GetCurrentColumn(); c < _gameManager.GetCurrentSentenceData().sentences.Count; c++)
        {
            if (!_gameManager.GetCorrectColumns().Contains(c))
            {
                correctTilesLeftTotal++;
            }
        }

        // Beer 아이템 사용 시, 정답이 100% 확실한 경우에만 선택
        if (_isBeerEffectActive)
        {
            if (availableIndices.Count == 1)
            {
                return new Position(availableIndices[0], currentColumn);
            }
            else
            {
                return new Position(-1, currentColumn); // 확실하지 않으면 선택 중지
            }
        }

        // Phase 2: 정답 추론 및 개수 결정 (Action Logic)
        if (correctTilesLeftTotal <= 2) // Case A
        {
            // 남은 정답 모두 선택 (여기서는 1개만 선택 가능하므로, 다음 턴에 나머지를 선택하게 됨)
            return new Position(availableIndices[0], currentColumn);
        }

        if (correctTilesLeftTotal == 3) // Case B
        {
            // 아이템 사용은 UseItem에서 처리. 여기서는 차선책으로 1개 선택
            return new Position(availableIndices[Random.Range(0, availableIndices.Count)], currentColumn);
        }

        int remainder = correctTilesLeftTotal % 3;
        int choicesToMake = 0;

        if (remainder == 1) // Case C (4개 남은 경우)
        {
            choicesToMake = 1;
        }
        else if (remainder == 2) // Case C (5개 남은 경우)
        {
            choicesToMake = 2;
        }
        else if (remainder == 0) // 이미 3의 배수 (6, 9개)
        {
            choicesToMake = 1; // 임의로 1개 선택하여 판 흔들기
        }

        // choicesToMake 만큼 선택해야 하지만, 현재 로직에서는 한 번에 하나만 선택 가능.
        // 일단 첫번째 선택을 하고, 남은 선택 기회가 있으면 추가 선택
        if (choicesToMake > 0 && availableIndices.Count > 0)
        {
             // Phase 3: 문장 완성 로직 (Consistency) - 현재는 승리 전략에 집중
            return new Position(availableIndices[0], currentColumn);
        }
        
        // 기본 로직: 랜덤 선택
        if (availableIndices.Count > 0)
        {
            return new Position(availableIndices[Random.Range(0, availableIndices.Count)], currentColumn);
        }

        return new Position(-1, currentColumn); // 선택할 게 없을 때 스킵
    }

    public ItemType UseItem()
    {
        // 1순위: 장갑
        if (inventory.ContainsKey(ItemType.Gloves) && inventory[ItemType.Gloves] > 0 && _gameManager.GetOpponent().GetTotalItems() > 0)
        {
            return ItemType.Gloves;
        }

        // 2순위: 돋보기 (정답 확신 시)
        int currentColumn = _gameManager.GetCurrentColumn();
        int availableCount = 0;
        for (int i = 0; i < _availableWords[currentColumn].Count; i++)
        {
            if (_availableWords[currentColumn][i])
            {
                availableCount++;
            }
        }
        if (availableCount == 2 && inventory.ContainsKey(ItemType.MagnifyingGlass) && inventory[ItemType.MagnifyingGlass] > 0)
        {
            return ItemType.MagnifyingGlass;
        }

        // 맥주 사용 조건 강화: 여러 개의 정답을 확실히 알 때만 사용
        if (inventory.ContainsKey(ItemType.Beer) && inventory[ItemType.Beer] > 0)
        {
            int certainCorrectColumns = 0;
            for (int c = _gameManager.GetCurrentColumn(); c < _gameManager.GetCurrentSentenceData().sentences.Count; c++)
            {
                if (_gameManager.GetCorrectColumns().Contains(c)) continue;

                int count = 0;
                for (int r = 0; r < _availableWords[c].Count; r++)
                {
                    if (_availableWords[c][r])
                    {
                        count++;
                    }
                }
                if (count == 1)
                {
                    certainCorrectColumns++;
                }
                else
                {
                    break; // 연속적이지 않으면 중단
                }
            }

            if (certainCorrectColumns >= 2) // 최소 2개 이상의 정답을 확신할 때
            {
                return ItemType.Beer;
            }
        }


        // 3순위: 아메리카노 (필승 구도가 아닐 때)
        int correctTilesLeft = 0;
        for (int c = _gameManager.GetCurrentColumn(); c < _gameManager.GetCurrentSentenceData().sentences.Count; c++)
        {
            if (!_gameManager.GetCorrectColumns().Contains(c))
            {
                correctTilesLeft++;
            }
        }

        if (correctTilesLeft % 3 == 0)
        {
            if (inventory.ContainsKey(ItemType.Americano) && inventory[ItemType.Americano] > 0)
            {
                return ItemType.Americano;
            }
        }

        // 4순위: 공개된 고문서 & 무전기 (정보 부족 시)
        // 문맥상 정답 유추가 어려운 경우를 판단하는 로직 추가 필요 (일단은 아이템이 있으면 사용)
        if (inventory.ContainsKey(ItemType.AncientDocument) && inventory[ItemType.AncientDocument] > 0)
        {
            return ItemType.AncientDocument;
        }
        if (inventory.ContainsKey(ItemType.Transceiver) && inventory[ItemType.Transceiver] > 0)
        {
            return ItemType.Transceiver;
        }

        return ItemType.None;
    }

    public void Initialize(GameManager gameManager, TurnManager turnManager)
    {
        _gameManager = gameManager;
        _turnManager = turnManager;
        _availableWords = new List<List<bool>>();
        foreach (var sentence in gameManager.GetCurrentSentenceData().sentences)
        {
            List<bool> availability = new List<bool>();
            for (int i = 0; i < sentence.Count; i++)
            {
                availability.Add(true);
            }
            _availableWords.Add(availability);
        }

    }

    public void RemoveAt(Position pos)
    {
        _availableWords[pos.col][pos.row] = false;
    }

    public void RemoveExceptAnswer(int column)
    {
        for(int row = 0; row < _availableWords[column].Count; row++)
        {
            if(_gameManager.GetCurrentSentenceData().sentences[column][row].isCorrect)
            {
                continue;
            }
            _availableWords[column][row] = false;
        }
    }
}