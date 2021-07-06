using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class KeyInputPanel : MonoBehaviour,IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void WhenOnClick();

    public WhenOnClick whenOnClick;

    public UnityEvent onEnter;
    public UnityEvent onExit;
    
    public void OnPointerClick(PointerEventData eventData) 
    {
        whenOnClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke();
    }
}
