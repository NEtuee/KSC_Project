using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class FromRightAppearButton : BaseAppearButton
{
    [SerializeField] private Vector2 _startOffset;
    public Vector2 StartOffset { get => _startOffset; set { _startOffset = value; } }

    [SerializeField] private float _duration;
    public float Duration { get => _duration; set => _duration = value; }
    [SerializeField] [Range(0.0f, 1.0f)] private float middleCallTiming = 0.5f;
    public float MiddleCallTiming { get => middleCallTiming; set => middleCallTiming = value; }
    private RectTransform _rect;
    private Vector2 _targetPosition;

    public UnityEvent onMiddle;

    protected override void Awake()
    {
        base.Awake();
        _rect = GetComponent<RectTransform>();
        if(_rect == null)
        {
            Debug.LogWarning("Not Exits RectTransform");
            return;
        }
    }

    public override void Init()
    {
        //Debug.Log("Init");
        interactable = false;
        _targetPosition = _rect.anchoredPosition;
        _rect.anchoredPosition += _startOffset;
        targetGraphic.DOFade(0.0f, 0.0f);
        //Appear();
    }

    public override void Appear()
    {
        if(onMiddle != null)
           StartCoroutine(Middle(_duration * middleCallTiming));

        _rect.DOAnchorPos(_targetPosition, _duration);
        targetGraphic.DOFade(1f, _duration).OnComplete(() => 
        {
            interactable = true;
        });
    }

    IEnumerator Middle(float time)
    {
        yield return new WaitForSeconds(time);
        onMiddle.Invoke();
    }
}
