using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class MainTitleButton : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private bool interactable = true;
    public bool Interactable { get => interactable; set => interactable = value; }


    public RectTransform buttonRect;
    public Image buttonImage;
    public TextMeshProUGUI text;
    public Vector2 beforeAnimationOffset;
    public float duration;
    [Range(0.0f,1.0f)]public float middleTriggerRatio;
    public Sprite defaultSprite;
    public Sprite selectedSprite;
    
    public UnityEvent onEndAppear;
    public UnityEvent onMiddle;

    private Vector2 _targetPosition;

    public UnityEvent onClick;
    
    private void Start()
    {
        Init();
    }

    public void Init()
    {
        //buttonRect.sizeDelta = startSize;
        Color alpha = buttonImage.color;
        alpha.a = 0;
        buttonImage.color = alpha;
        if (text != null)
        {
            alpha = text.color;
            alpha.a = 0;
            text.color = alpha;
        }

        _targetPosition = buttonRect.anchoredPosition;
        buttonRect.anchoredPosition += beforeAnimationOffset;

        middleTriggerRatio = Mathf.Clamp(middleTriggerRatio, 0.0f, 1.0f);
    }

    public void Appear()
    {
        StartCoroutine(Middle(duration * middleTriggerRatio));
        //buttonRect.DOSizeDelta(targetSize, duration);
        buttonRect.DOAnchorPos(_targetPosition, duration);
        buttonImage.DOFade(1f, duration).OnComplete(() => { onEndAppear?.Invoke(); });
        if (text != null)
            text.DOFade(1f, duration);
        
    }

    IEnumerator Middle(float time)
    {
        yield return new WaitForSeconds(time);
        onMiddle?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (interactable == false)
            return;
        buttonImage.sprite = selectedSprite;

        GameManager.Instance.soundManager.Play(3000, Vector3.zero);
        GameManager.Instance.soundManager.SetParam(3000, 30001, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (interactable == false)
            return;
        buttonImage.sprite = defaultSprite;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (interactable == false)
            return;
        onClick?.Invoke();

        GameManager.Instance.soundManager.Play(3000, Vector3.zero);
        GameManager.Instance.soundManager.SetParam(3000, 30001, 1);
    }
}
