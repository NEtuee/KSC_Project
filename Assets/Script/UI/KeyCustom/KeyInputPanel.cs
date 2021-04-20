using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyInputPanel : MonoBehaviour,IPointerClickHandler
{
    public delegate void WhenOnClick();

    public WhenOnClick whenOnClick;
    
    public void OnPointerClick(PointerEventData eventData) 
    {
        whenOnClick?.Invoke();
    }
}
