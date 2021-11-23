using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainMenuAppearUI : BaseAppearButton
{
    private RectTransform _rect;

    protected override void Awake()
    {
        base.Awake();
        _rect = GetComponent<RectTransform>();
        if (_rect == null)
        {
            Debug.LogWarning("Not Exits RectTransform");
            return;
        }
    }

    public override void Init()
    {
        interactable = false;
        targetGraphic.DOFade(0.0f, 0.0f);
    }
}
