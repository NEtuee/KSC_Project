using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TextButton : BaseAppearButton
{
    public TextMeshProUGUI targetText;

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        //targetText.color = colors.highlightedColor;
        onSeleted.Invoke();
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        //targetText.color = colors.disabledColor;
        onDeseleted.Invoke();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        //targetText.color = colors.highlightedColor;
        onEnter.Invoke();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        //targetText.color = colors.disabledColor;
        onExit.Invoke();
    }
}
