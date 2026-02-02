using System.Collections.Generic;

[System.Serializable]
public class SentenceData
{
    public List<List<WordData>> sentences;

    public SentenceData()
    {
        sentences = new List<List<WordData>>();
    }

    public SentenceData(SentenceData other)
    {
        this.sentences = new List<List<WordData>>();
        foreach (var sentence in other.sentences)
        {
            var newSentence = new List<WordData>();
            foreach (var wordData in sentence)
            {
                newSentence.Add(new WordData(wordData));
            }
            this.sentences.Add(newSentence);
        }
    }
}
