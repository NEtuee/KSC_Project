using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class CrossHair : MonoBehaviour
{
    private Canvas _canvas;

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

        left.DOAnchorPosX(-sidePosX, 0.5f);
        right.DOAnchorPosX(sidePosX, 0.5f);
        left.DOScale(sideScale, 0.5f);
        right.DOScale(sideScale, 0.5f);
    }

    public void Init()
    {
        left.anchoredPosition = new Vector2(-centerPosX, left.anchoredPosition.y);
        right.anchoredPosition = new Vector2(-centerPosX, right.anchoredPosition.y);

        stage = 0;
    }

    public void First()
    {
        if (stage != 0)
            return;

        left.DOAnchorPosX(-middlePosX, 0.2f);
        right.DOAnchorPosX(middlePosX, 0.2f);
        left.DOScale(middleScale, 0.2f);
        right.DOScale(middleScale, 0.2f);
        stage = 1;
    }

    public void Second()
    {
        if (stage != 1)
            return;

        left.DOAnchorPosX(-centerPosX, 0.2f);
        right.DOAnchorPosX(centerPosX, 0.2f);
        left.DOScale(centerScale, 0.2f);
        right.DOScale(centerScale, 0.2f);
        stage = 2;
    }
}
