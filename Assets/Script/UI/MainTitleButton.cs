using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

public class MainTitleButton : MonoBehaviour
{
    public RectTransform buttonRect;
    public Image buttonImage;
    public TextMeshProUGUI text;
    public Vector2 targetSize;
    public Vector2 startSize;
    public float duration;
    public UnityEvent OnEndAppear;
    public UnityEvent OnMiddle;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        buttonRect.sizeDelta = startSize;
        Color alpha = buttonImage.color;
        alpha.a = 0;
        buttonImage.color = alpha;
        alpha = text.color;
        alpha.a = 0;
        text.color = alpha;
    }

    public void Appear()
    {
        StartCoroutine(Middle(duration * 0.5f));
        buttonRect.DOSizeDelta(targetSize, duration);
        buttonImage.DOFade(1f, duration);
        text.DOFade(1f, duration).OnComplete(() => { OnEndAppear?.Invoke(); });
    }

    IEnumerator Middle(float time)
    {
        yield return new WaitForSeconds(time);
        OnMiddle?.Invoke();
    }
}
