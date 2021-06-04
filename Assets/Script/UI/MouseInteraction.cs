using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MouseInteraction : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public UnityEvent onEnter;
    public UnityEvent onExit;

    public void OnPointerEnter(PointerEventData eventData)
    {
        onEnter?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onExit?.Invoke();
    }
}
