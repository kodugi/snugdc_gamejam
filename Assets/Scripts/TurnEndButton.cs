using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurnEndButton : MonoBehaviour,IPointerClickHandler
{
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    } public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
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
