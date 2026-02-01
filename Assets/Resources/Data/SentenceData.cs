using System.Collections.Generic;

[System.Serializable]
public class SentenceData
{
    public List<List<List<WordData>>> sentences;
    public List<List<int>> answers;
}
