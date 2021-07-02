using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BaseAppearImage : MonoBehaviour, Appearable
{
    [SerializeField] protected Image targetImage;

    protected void Awake()
    {
        targetImage = GetComponent<Image>();
        if (targetImage == null)
        {
            Debug.LogWarning("Not Exist Image Component");
            return;
        }

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

    public void Init()
    {
    }
}
