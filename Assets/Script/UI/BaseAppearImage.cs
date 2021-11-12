using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BaseAppearImage : MonoBehaviour, Appearable
{
    [SerializeField] protected Image targetImage;
    [SerializeField] protected RectTransform rectTransform;

    [SerializeField] protected UnityEvent whenEndAppear;
    [SerializeField] protected UnityEvent whenEndDisappear;

    public RectTransform RectTransform => rectTransform;
    public Image Image => targetImage;

    protected void Awake()
    {
        targetImage = GetComponent<Image>();
        if (targetImage == null)
        {
            Debug.LogWarning("Not Exist Image Component");
            return;
        }

        rectTransform = GetComponent<RectTransform>();

        targetImage.raycastTarget = false;
    }

    protected void Start()
    {
    }

    public virtual void Appear()
    {
    }

    public virtual void Disappear()
    {
    }

    public virtual void Init()
    {
    }
}
