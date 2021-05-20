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

    public float centerOffset = 190.0f;
    public float firstStageOffset = 600.0f;
    public float secondStageOffset = 400.0f;

    public RectTransform left;
    public RectTransform right;

    public Image leftImage;
    public Image rightImage;

    public float centerPosX = 12.5f;
    public float centerScale = 1;
    public float middlePosX = 100.0f;
    public float middleScale = 1.5f;
    public float sidePosX = 200.0f;
    public float sideScale = 2.0f;

    private int stage = 0;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        SetActive(false);
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            ActiveAnimation();
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

        leftCross.DOAnchorPosX(-firstStageOffset, 0.5f);
        rightCross.DOAnchorPosX(firstStageOffset, 0.5f);
        upCross.DOAnchorPosY(firstStageOffset, 0.5f);
        downCross.DOAnchorPosY(-firstStageOffset, 0.5f);

        leftUpLine.DOAnchorPos(new Vector2(-firstStageOffset, firstStageOffset), 0.5f);
        leftDownLine.DOAnchorPos(new Vector2(-firstStageOffset, -firstStageOffset), 0.5f);
        RightUpLine.DOAnchorPos(new Vector2(firstStageOffset, firstStageOffset), 0.5f);
        RightDownLine.DOAnchorPos(new Vector2(firstStageOffset, -firstStageOffset), 0.5f);
    }

    public void Init()
    {
        //left.anchoredPosition = new Vector2(-centerPosX, left.anchoredPosition.y);
        //right.anchoredPosition = new Vector2(-centerPosX, right.anchoredPosition.y);

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

    public void First()
    {
        if (stage != 0)
            return;

        //left.DOAnchorPosX(-middlePosX, 0.2f);
        //right.DOAnchorPosX(middlePosX, 0.2f);
        //left.DOScale(middleScale, 0.2f);
        //right.DOScale(middleScale, 0.2f);

        leftCross.DOAnchorPosX(-secondStageOffset, 0.2f);
        rightCross.DOAnchorPosX(secondStageOffset, 0.2f);
        upCross.DOAnchorPosY(secondStageOffset, 0.2f);
        downCross.DOAnchorPosY(-secondStageOffset, 0.2f);

        leftUpLine.DOAnchorPos(new Vector2(-secondStageOffset, secondStageOffset), 0.2f);
        leftDownLine.DOAnchorPos(new Vector2(-secondStageOffset, -secondStageOffset), 0.2f);
        RightUpLine.DOAnchorPos(new Vector2(secondStageOffset, secondStageOffset), 0.2f);
        RightDownLine.DOAnchorPos(new Vector2(secondStageOffset, -secondStageOffset), 0.2f);

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

        leftCross.DOAnchorPosX(-centerOffset, 0.2f);
        rightCross.DOAnchorPosX(centerOffset, 0.2f);
        upCross.DOAnchorPosY(centerOffset, 0.2f);
        downCross.DOAnchorPosY(-centerOffset, 0.2f);

        leftUpLine.DOAnchorPos(new Vector2(-centerOffset, centerOffset), 0.2f);
        leftDownLine.DOAnchorPos(new Vector2(-centerOffset, -centerOffset), 0.2f);
        RightUpLine.DOAnchorPos(new Vector2(centerOffset, centerOffset), 0.2f);
        RightDownLine.DOAnchorPos(new Vector2(centerOffset, -centerOffset), 0.2f);

        stage = 2;
    }
}
