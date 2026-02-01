using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurnEndButton : MonoBehaviour
{
   Button _button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnClick()
    {
        Debug.Log("turnEndclicked");
        GameManager.Instance.TurnEnd();
    }

    public void Enable()
    {
        GetComponent<Image>().raycastTarget = true;
        GetComponent<Image>().color = Color.white;
    }

    public void Disable()
    {
        GetComponent<Image>().raycastTarget = false;
        GetComponent<Image>().color = Color.dimGray;
        
    }
}
