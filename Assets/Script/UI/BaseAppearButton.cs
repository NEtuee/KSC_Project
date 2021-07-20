using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BaseAppearButton : Button, Appearable
{
    public UnityEvent onSeleted;
    public UnityEvent onDeseleted;
    public UnityEvent onEnter;
    public UnityEvent onExit;

    protected override void Start()
    {
        base.Start();
    }

    public virtual void Appear()
    {
    }

    public virtual void Disappear()
    {
    }

    public virtual void Init()
    {
    }

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
