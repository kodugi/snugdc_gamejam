using UnityEngine;
using System.Collections.Generic;

public class SentenceParser : MonoBehaviour
{
    [System.Serializable]
    private class WordListWrapper
    {
        public List<WordData> words;
    }

    [System.Serializable]
    private class SentenceListWrapper
    {
        public List<WordListWrapper> sentences;
    }

    [System.Serializable]
    private class SentenceDataWrapper
    {
        public List<SentenceListWrapper> stages;
    }

    public List<SentenceData> sentenceDataList;

    void Awake()
    {
        TextAsset sentenceJson = Resources.Load<TextAsset>("StageData/Sentences");
        if (sentenceJson != null)
        {
            SentenceDataWrapper wrapper = JsonUtility.FromJson<SentenceDataWrapper>(sentenceJson.text);
            sentenceDataList = new List<SentenceData>();

            foreach (var stage in wrapper.stages)
            {
                SentenceData data = new SentenceData
                {
                    sentences = new List<List<WordData>>()
                };

                foreach (var sentence in stage.sentences)
                {
                    data.sentences.Add(sentence.words);
                }
                sentenceDataList.Add(data);
            }
        }
        else
        {
            Debug.LogError("Cannot find file Sentences.json in Resources/StageData");
        }
    }
}
