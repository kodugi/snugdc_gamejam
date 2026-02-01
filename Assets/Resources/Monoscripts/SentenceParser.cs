using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SentenceParser : MonoBehaviour
{
    [System.Serializable]
    private class SentenceDataWrapper
    {
        public List<List<List<WordData>>> sentences;
    }

    public List<SentenceData> sentenceDataList;

    void Awake()
    {
        TextAsset sentenceJson = Resources.Load<TextAsset>("StageData/Sentences");
        if (sentenceJson != null)
        {
            var wrapper = JsonConvert.DeserializeObject<SentenceDataWrapper>(sentenceJson.text);
            sentenceDataList = new List<SentenceData>();

            for (int i = 0; i < wrapper.sentences.Count; i++)
            {
                SentenceData data = new SentenceData
                {
                    sentences = wrapper.sentences[i]
                };
                sentenceDataList.Add(data);
            }
        }
        else
        {
            Debug.LogError("Cannot find file Sentences.json in Resources/StageData");
        }
    }
}
