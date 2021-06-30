using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleUi : BaseUi
{
    private Image _targetImage;

    protected void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {
        _targetImage = GetComponent<Image>();
        if(_targetImage == null)
        {
            Debug.LogError("Not Exits TargetImage");
            return;
        }
        _targetImage.raycastTarget = false;
    }

    public override void Appear()
    {
        base.Appear();
        endAppear.Invoke();
    }

    public override void Disappear()
    {
        base.Disappear();
        endDisappear.Invoke();
    }
}
