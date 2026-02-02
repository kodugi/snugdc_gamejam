using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;
    [SerializeField] private Image _image;

    private float failsafeTimer=0f;
    private float timerEnd = 10f;
    public void Disable()
    {
        this.transform.gameObject.SetActive(false);
    }

    private void Enable()
    {
        this.transform.gameObject.SetActive(true);
    }

    public void Init(string text, Sprite sprite, float time=10f)
    {
        _textMeshProUGUI.text = text;
        _image.sprite = sprite;
        timerEnd = time;
        Enable();
    }

    public void OnEnable()
    {
        failsafeTimer = 0f;
    }

    void Update()
    {
        failsafeTimer += Time.deltaTime;
        if (failsafeTimer >= timerEnd)
        {
            Disable();
        }
    }
}
