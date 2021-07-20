using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextButtonWithBackGround : TextButton
{
    public Image backGroundImage;
    public Color selectColor;
    public Color enterColor;
    public Color deselectColor;
    public Color exitColor;

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        backGroundImage.color = selectColor;
        onSeleted.Invoke();
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        backGroundImage.color = deselectColor;
        onDeseleted.Invoke();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        backGroundImage.color = enterColor;
        onEnter.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        backGroundImage.color = exitColor;
        onExit.Invoke();
    }
}
