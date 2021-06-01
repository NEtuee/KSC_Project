using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeftOptionTitle : EscMenu
{
    private Canvas _canvas;
    public RectTransform imageRect;
    public TextMeshProUGUI optionMenuText;
    public float initRectXpos = -2162f;
    public float appearRectXpos = -1680f;
    public float changeRectXpos;
    public string optionTitleString = "일시정지";

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        Vector2 pos = imageRect.anchoredPosition;
        imageRect.anchoredPosition = new Vector2(initRectXpos, pos.y);
    }

    public override void Appear(float duration)
    {
        _canvas.enabled = true;
        imageRect.DOAnchorPosX(appearRectXpos, duration);
    }

    public override void Appear(float duration, TweenCallback tweenCallback)
    {
        _canvas.enabled = true;
        imageRect.DOAnchorPosX(appearRectXpos, duration).OnComplete(tweenCallback);
    }

    public override void Disappear(float duration)
    {
        imageRect.DOAnchorPosX(initRectXpos, duration).OnComplete(()=>_canvas.enabled = false);
    }

    public override void Disappear(float duration, TweenCallback tweenCallback)
    {
        imageRect.DOAnchorPosX(initRectXpos, duration).OnComplete(() => { tweenCallback(); _canvas.enabled = false; });
    }

    public void SetEnableCanvas(bool active)
    {
        _canvas.enabled = active;
    }

    public void ChangeOption(OptionMenuCtrl.MenuType menuType,Action changeMenu, float changeDuration = 0.5f)
    {
        float waitTime = changeDuration * 0.3f;
        float tweenTime = (changeDuration - waitTime) * 0.5f;

        imageRect.DOAnchorPosX(changeRectXpos, tweenTime).OnComplete(()=>
        {
            if(optionMenuText != null)
            {
                switch(menuType)
                {
                    case OptionMenuCtrl.MenuType.Sound:
                        optionMenuText.text = "사운드";
                        break;
                    case OptionMenuCtrl.MenuType.Control:
                        optionMenuText.text = "조작";
                        break;
                    case OptionMenuCtrl.MenuType.Display:
                        optionMenuText.text = "디스플레이";
                        break;
                    case OptionMenuCtrl.MenuType.Key:
                        optionMenuText.text = "키 바인딩";
                        break;
                    case OptionMenuCtrl.MenuType.Option:
                        optionMenuText.text = optionTitleString;
                        break;
                }
            }
            changeMenu?.Invoke();
            imageRect.DOAnchorPosX(appearRectXpos, tweenTime).SetDelay(waitTime).OnComplete(()=> 
            {
            });
        });
    }

    public override void Active(bool active)
    {
        _canvas.enabled = active;
    }

    public override void Init()
    {
        throw new NotImplementedException();
    }
}
