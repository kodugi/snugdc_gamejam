using Newtonsoft.Json;

[System.Serializable]
public class WordData
{
    public string word;
    public WordType type;
    public bool isCorrect;

    [JsonConstructor]
    public WordData() { }

    public WordData(WordData other)
    {
        this.word = other.word;
        this.type = other.type;
        this.isCorrect = other.isCorrect;
    }
}