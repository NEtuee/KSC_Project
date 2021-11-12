using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class CrossHair : MonoBehaviour
{
    private Canvas _canvas;

    public RectTransform leftCross;
    public RectTransform rightCross;
    public RectTransform upCross;
    public RectTransform downCross;

    public RectTransform leftUpLine;
    public RectTransform leftDownLine;
    public RectTransform RightUpLine;
    public RectTransform RightDownLine;

    public Image hitMark;
    private Color hitMarkColor;

    public float centerOffset = 190.0f;
    public float firstStageOffset = 600.0f;
    public float secondStageOffset = 250.0f;
    public float thirdStageOffset = 350.0f;

    public float centerPosX = 12.5f;
    public float centerScale = 1;
    public float middlePosX = 100.0f;
    public float middleScale = 1.5f;
    public float sidePosX = 200.0f;
    public float sideScale = 2.0f;

    private int stage = 0;

    private Vector2 aimBackGrounOriginSize;
    [SerializeField] private RectTransform aimBackGround;
    [SerializeField] private RectTransform leftGrid;
    [SerializeField] private RectTransform rightGrid;

    [SerializeField] private float backGroundOriginalWidth;
    [SerializeField] private float backGroundEffectWidth;
    [SerializeField] private float leftGridOriginPosX;
    [SerializeField] private float leftGridEffectPosX;
    [SerializeField] private float rightGridOriginPosX;
    [SerializeField] private float rightGridEffectPosX;

    private bool _chargeComplete = false;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        SetActive(false);
        hitMarkColor = hitMark.color;
        hitMark.color = new Color(hitMarkColor.r, hitMarkColor.g, hitMarkColor.b, 0.0f);
        aimBackGrounOriginSize = aimBackGround.sizeDelta;
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            ActiveAnimation();
        }
        else
        {
            _chargeComplete = false;
        }

        _canvas.enabled = active;
    }

    public void ActiveAnimation()
    {
        Init();

        //left.DOAnchorPosX(-sidePosX, 0.5f);
        //right.DOAnchorPosX(sidePosX, 0.5f);
        //left.DOScale(sideScale, 0.5f);
        //right.DOScale(sideScale, 0.5f);

        // leftCross.DOAnchorPosX(-firstStageOffset, 0.5f);
        // rightCross.DOAnchorPosX(firstStageOffset, 0.5f);
        // upCross.DOAnchorPosY(firstStageOffset, 0.5f);
        // downCross.DOAnchorPosY(-firstStageOffset, 0.5f);
        //
        // leftUpLine.DOAnchorPos(new Vector2(-firstStageOffset, firstStageOffset), 0.5f);
        // leftDownLine.DOAnchorPos(new Vector2(-firstStageOffset, -firstStageOffset), 0.5f);
        // RightUpLine.DOAnchorPos(new Vector2(firstStageOffset, firstStageOffset), 0.5f);
        // RightDownLine.DOAnchorPos(new Vector2(firstStageOffset, -firstStageOffset), 0.5f);

        aimBackGround.DOSizeDelta(new Vector2(backGroundOriginalWidth, aimBackGrounOriginSize.y), 0.15f).SetUpdate(true).SetEase(Ease.OutBack);
        leftGrid.DOAnchorPosX(leftGridOriginPosX, 0.15f).SetUpdate(true).SetEase(Ease.OutBack);
        rightGrid.DOAnchorPosX(rightGridOriginPosX, 0.15f).SetUpdate(true).SetEase(Ease.OutBack);

        float speed = 0.15f;
        
        leftCross.DOAnchorPosX(-firstStageOffset, speed).SetUpdate(true).SetEase(Ease.OutBack);
        rightCross.DOAnchorPosX(firstStageOffset, speed).SetUpdate(true).SetEase(Ease.OutBack);
        upCross.DOAnchorPosY(firstStageOffset, speed).SetUpdate(true).SetEase(Ease.OutBack);
        downCross.DOAnchorPosY(-firstStageOffset, speed).SetUpdate(true).SetEase(Ease.OutBack);
        
        leftUpLine.DOAnchorPos(new Vector2(-firstStageOffset, firstStageOffset), speed).SetUpdate(true);
        leftDownLine.DOAnchorPos(new Vector2(-firstStageOffset, -firstStageOffset), speed).SetUpdate(true);
        RightUpLine.DOAnchorPos(new Vector2(firstStageOffset, firstStageOffset), speed).SetUpdate(true);
        RightDownLine.DOAnchorPos(new Vector2(firstStageOffset, -firstStageOffset), speed).SetUpdate(true);
    }

    public void Init()
    {
        //left.anchoredPosition = new Vector2(-centerPosX, left.anchoredPosition.y);
        //right.anchoredPosition = new Vector2(-centerPosX, right.anchoredPosition.y);

        //aimBackGround.DOSizeDelta(new Vector2(backGroundEffectWidth, aimBackGrounOriginSize.y), 0.5f).SetUpdate(true).SetEase(Ease.OutBack);
        aimBackGround.sizeDelta = new Vector2(backGroundEffectWidth, aimBackGrounOriginSize.y);
        leftGrid.DOAnchorPosX(leftGridEffectPosX, 0.0f).SetUpdate(true);
        rightGrid.DOAnchorPosX(rightGridEffectPosX, 0.0f).SetUpdate(true);


        leftCross.anchoredPosition = new Vector2(-centerOffset,0.0f);
        rightCross.anchoredPosition = new Vector2(centerOffset, 0.0f);
        upCross.anchoredPosition = new Vector2(0.0f, centerOffset);
        downCross.anchoredPosition = new Vector2(0.0f, -centerOffset);

        leftUpLine.anchoredPosition = new Vector2(-centerOffset, centerOffset);
        leftDownLine.anchoredPosition = new Vector2(-centerOffset, -centerOffset);
        RightUpLine.anchoredPosition = new Vector2(centerOffset, centerOffset);
        RightDownLine.anchoredPosition = new Vector2(centerOffset, -centerOffset);

        stage = 0;
    }

    public void Launch()
    {
        //leftCross.DOAnchorPosX(-thirdStageOffset, 0.15f);
        //rightCross.DOAnchorPosX(thirdStageOffset, 0.15f);
        //upCross.DOAnchorPosY(thirdStageOffset, 0.15f);
        //downCross.DOAnchorPosY(-thirdStageOffset, 0.15f);

        //leftUpLine.DOAnchorPos(new Vector2(-thirdStageOffset, thirdStageOffset), 0.15f).SetEase(Ease.OutQuart);
        //leftDownLine.DOAnchorPos(new Vector2(-thirdStageOffset, -thirdStageOffset), 0.15f).SetEase(Ease.OutQuart);
        //RightUpLine.DOAnchorPos(new Vector2(thirdStageOffset, thirdStageOffset), 0.15f).SetEase(Ease.OutQuart);
        //RightDownLine.DOAnchorPos(new Vector2(thirdStageOffset, -thirdStageOffset), 0.15f).SetEase(Ease.OutQuart);

        aimBackGround.DOSizeDelta(new Vector2(backGroundOriginalWidth, aimBackGrounOriginSize.y), 0.15f).SetUpdate(true).SetEase(Ease.OutBack);
        leftGrid.DOAnchorPosX(leftGridOriginPosX, 0.15f).SetUpdate(true).SetEase(Ease.OutBack);
        rightGrid.DOAnchorPosX(rightGridOriginPosX, 0.15f).SetUpdate(true).SetEase(Ease.OutBack).OnComplete(()=>
        {
            _chargeComplete = false;
        });

        float speed = 0.15f;

        leftCross.DOAnchorPosX(-firstStageOffset, speed).SetUpdate(true).SetEase(Ease.OutBack);
        rightCross.DOAnchorPosX(firstStageOffset, speed).SetUpdate(true).SetEase(Ease.OutBack);
        upCross.DOAnchorPosY(firstStageOffset, speed).SetUpdate(true).SetEase(Ease.OutBack);
        downCross.DOAnchorPosY(-firstStageOffset, speed).SetUpdate(true).SetEase(Ease.OutBack);

        leftUpLine.DOAnchorPos(new Vector2(-firstStageOffset, firstStageOffset), speed).SetUpdate(true);
        leftDownLine.DOAnchorPos(new Vector2(-firstStageOffset, -firstStageOffset), speed).SetUpdate(true);
        RightUpLine.DOAnchorPos(new Vector2(firstStageOffset, firstStageOffset), speed).SetUpdate(true);
        RightDownLine.DOAnchorPos(new Vector2(firstStageOffset, -firstStageOffset), speed).SetUpdate(true).OnComplete(() => stage = 0);

 
    }

    public void ChargeComplete()
    {
        if(_chargeComplete == false)
        {
            _chargeComplete = true;
            aimBackGround.DOSizeDelta(new Vector2(backGroundEffectWidth, aimBackGrounOriginSize.y), 0.15f).SetUpdate(true).SetEase(Ease.OutBack);
            leftGrid.DOAnchorPosX(leftGridEffectPosX, 0.15f).SetUpdate(true).SetEase(Ease.OutBack);
            rightGrid.DOAnchorPosX(rightGridEffectPosX, 0.15f).SetUpdate(true).SetEase(Ease.OutBack);
        }
    }

    public void First()
    {
        if (stage != 0)
            return;
        //left.DOAnchorPosX(-middlePosX, 0.2f);
        //right.DOAnchorPosX(middlePosX, 0.2f);
        //left.DOScale(middleScale, 0.2f);
        //right.DOScale(middleScale, 0.2f);

        leftCross.DOAnchorPosX(-thirdStageOffset, 0.2f);
        rightCross.DOAnchorPosX(thirdStageOffset, 0.2f);
        upCross.DOAnchorPosY(thirdStageOffset, 0.2f);
        downCross.DOAnchorPosY(-thirdStageOffset, 0.2f);

        leftUpLine.DOAnchorPos(new Vector2(-thirdStageOffset, thirdStageOffset), 0.2f).SetEase(Ease.OutQuart);
        leftDownLine.DOAnchorPos(new Vector2(-thirdStageOffset, -thirdStageOffset), 0.2f).SetEase(Ease.OutQuart);
        RightUpLine.DOAnchorPos(new Vector2(thirdStageOffset, thirdStageOffset), 0.2f).SetEase(Ease.OutQuart);
        RightDownLine.DOAnchorPos(new Vector2(thirdStageOffset, -thirdStageOffset), 0.2f).SetEase(Ease.OutQuart);

        stage = 1;
    }

    public void Second()
    {
        if (stage != 1)
            return;

        //left.DOAnchorPosX(-centerPosX, 0.2f);
        //right.DOAnchorPosX(centerPosX, 0.2f);
        //left.DOScale(centerScale, 0.2f);
        //right.DOScale(centerScale, 0.2f);

        leftCross.DOAnchorPosX(-secondStageOffset, 0.2f);
        rightCross.DOAnchorPosX(secondStageOffset, 0.2f);
        upCross.DOAnchorPosY(secondStageOffset, 0.2f);
        downCross.DOAnchorPosY(-secondStageOffset, 0.2f);

        leftUpLine.DOAnchorPos(new Vector2(-secondStageOffset, secondStageOffset), 0.2f).SetEase(Ease.OutQuart);
        leftDownLine.DOAnchorPos(new Vector2(-secondStageOffset, -secondStageOffset), 0.2f).SetEase(Ease.OutQuart);
        RightUpLine.DOAnchorPos(new Vector2(secondStageOffset, secondStageOffset), 0.2f).SetEase(Ease.OutQuart);
        RightDownLine.DOAnchorPos(new Vector2(secondStageOffset, -secondStageOffset), 0.2f).SetEase(Ease.OutQuart);

        stage = 2;
    }

    public void Third()
    {
        if (stage != 2)
            return;

        //left.DOAnchorPosX(-centerPosX, 0.2f);
        //right.DOAnchorPosX(centerPosX, 0.2f);
        //left.DOScale(centerScale, 0.2f);
        //right.DOScale(centerScale, 0.2f);

        leftCross.DOAnchorPosX(-centerOffset, 0.2f);
        rightCross.DOAnchorPosX(centerOffset, 0.2f);
        upCross.DOAnchorPosY(centerOffset, 0.2f);
        downCross.DOAnchorPosY(-centerOffset, 0.2f);

        leftUpLine.DOAnchorPos(new Vector2(-centerOffset, centerOffset), 0.2f).SetEase(Ease.OutQuart);
        leftDownLine.DOAnchorPos(new Vector2(-centerOffset, -centerOffset), 0.2f).SetEase(Ease.OutQuart);
        RightUpLine.DOAnchorPos(new Vector2(centerOffset, centerOffset), 0.2f).SetEase(Ease.OutQuart);
        RightDownLine.DOAnchorPos(new Vector2(centerOffset, -centerOffset), 0.2f).SetEase(Ease.OutQuart);

        stage = 3;
    }

    public void ActiveHitMark()
    {
        hitMark.color = hitMarkColor;
        hitMark.DOFade(0.0f, 0.3f).SetDelay(0.3f);
    }
}
