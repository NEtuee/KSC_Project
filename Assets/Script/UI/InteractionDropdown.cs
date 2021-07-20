using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InteractionDropdown : TMP_Dropdown
{
    public UnityEvent onSeleted;
    public UnityEvent onDeseleted;
    public UnityEvent onEnter;
    public UnityEvent onExit;

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        onSeleted.Invoke();
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        onDeseleted.Invoke();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        onEnter.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        onExit.Invoke();
    }
}
