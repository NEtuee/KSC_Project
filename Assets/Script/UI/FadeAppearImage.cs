using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FadeAppearImage : BaseAppearImage
{
    public enum FadeType
    {
        ColorFade,
        ScaleFade,
        FillFade,
        PosXFade
    }

    public List<FadeType> types = new List<FadeType>();

    [Header("Color")]
    [SerializeField] private float appearDuration = 2f;
    [SerializeField] private float appearTarget = 1f;
    [SerializeField] private float disappearDuration = 2f;
    [SerializeField] private float disappearTarget = 0f;

    [SerializeField] private bool awakeVisble = false;

    [Header("Scale")]
    [SerializeField] private Vector3 appearScale;
    [SerializeField] private Vector3 disappearScale;

    [Header("Fill")]
    [SerializeField] private float appearFillAmount = 1f;
    [SerializeField] private float disappearFillAmount = 0f;

    [Header("PosX")]
    [SerializeField] private Ease ease;
    [SerializeField] private float appearPosX;
    [SerializeField] private float disappearPosX;

    public float AppearDuration => appearDuration;
    public float DisappearDuration => disappearDuration;

    private delegate void AppearDelegate();
    private AppearDelegate appearDelegate;

    private delegate void DisappearDelegate();
    private DisappearDelegate disappearDelegate;

    protected new void Awake()
    {
        base.Awake();

        for(int i = 0; i < types.Count; i++)
        {
            if(types[i] == FadeType.ColorFade)
            {
                if(i == types.Count - 1)
                {
                    appearDelegate += () =>
                    {
                        targetImage.DOFade(appearTarget, appearDuration).OnComplete(() =>
                        {
                            whenEndAppear.Invoke();
                        });
                    };
                    disappearDelegate += () =>
                    {
                        targetImage.DOFade(disappearTarget, disappearDuration).OnComplete(() =>
                        {
                            whenEndDisappear.Invoke();
                        });
                    };
                }   
                else
                {
                    appearDelegate += () =>
                    {
                        targetImage.DOFade(appearTarget, appearDuration);
                    };
                    disappearDelegate += () =>
                    {
                        targetImage.DOFade(disappearTarget, disappearDuration);
                    };
                }
            }
            else if (types[i] == FadeType.ScaleFade)
            {
                if (i == types.Count - 1)
                {
                    appearDelegate += () =>
                    {
                        targetImage.rectTransform.DOScale(appearScale, appearDuration).OnComplete(() =>
                        {
                            whenEndAppear.Invoke();
                        });
                    };
                    disappearDelegate += () =>
                    {
                        targetImage.rectTransform.DOScale(disappearScale, disappearDuration).OnComplete(() =>
                        {
                            whenEndDisappear.Invoke();
                        });
                    };
                }
                else
                {
                    appearDelegate += () =>
                    {
                        targetImage.rectTransform.DOScale(appearScale, appearDuration);
                    };
                    disappearDelegate += () =>
                    {
                        targetImage.rectTransform.DOScale(disappearScale, disappearDuration);
                    };
                }
            }
            else if (types[i] == FadeType.FillFade)
            {
                if (i == types.Count - 1)
                {
                    appearDelegate += () =>
                    {
                        targetImage.DOFillAmount(appearFillAmount, appearDuration).OnComplete(() =>
                        {
                            whenEndAppear.Invoke();
                        });
                    };
                    disappearDelegate += () =>
                    {
                        targetImage.DOFillAmount(disappearFillAmount, disappearDuration).OnComplete(() =>
                        {
                            whenEndDisappear.Invoke();
                        });
                    };
                }
                else
                {
                    appearDelegate += () =>
                    {
                        targetImage.DOFillAmount(appearFillAmount, appearDuration);
                    };
                    disappearDelegate += () =>
                    {
                        targetImage.DOFillAmount(disappearFillAmount, disappearDuration);
                    };
                }
            }
            else if(types[i] == FadeType.PosXFade)
            {
                if (i == types.Count - 1)
                {
                    appearDelegate += () =>
                    {
                        rectTransform.DOAnchorPosX(appearPosX, appearDuration).SetEase(ease).OnComplete(() =>
                        {
                            whenEndAppear.Invoke();
                        });
                    };
                    disappearDelegate += () =>
                    {
                        rectTransform.DOAnchorPosX(disappearPosX, disappearDuration).SetEase(ease).OnComplete(() =>
                        {
                            whenEndDisappear.Invoke();
                        });
                    };
                }
                else
                {
                    appearDelegate += () =>
                    {
                        rectTransform.DOAnchorPosX(appearPosX, appearDuration).SetEase(ease);
                    };
                    disappearDelegate += () =>
                    {
                        rectTransform.DOAnchorPosX(disappearPosX, disappearDuration).SetEase(ease);
                    };
                }
            }
        }
    }

    protected new void Start()
    {
        base.Start();

     
        for (int i = 0; i < types.Count; i++)
        {
            if (types[i] == FadeType.ColorFade)
            {
                if (awakeVisble == true)
                {
                    var color = targetImage.color;
                    color.a = appearTarget;
                    targetImage.color = color;
                }
                else
                {
                    var color = targetImage.color;
                    color.a = disappearTarget;
                    targetImage.color = color;
                }
            }
            else if (types[i] == FadeType.ScaleFade)
            {
                if (awakeVisble == true)
                {
                    targetImage.rectTransform.localScale = appearScale;
                }
                else
                {
                    targetImage.rectTransform.localScale = disappearScale;
                }
            }
            else if (types[i] == FadeType.FillFade)
            {
                if (awakeVisble == true)
                {
                    targetImage.fillAmount = appearFillAmount;
                }
                else
                {
                    targetImage.fillAmount = disappearFillAmount;
                }
            }
            else if(types[i] == FadeType.PosXFade)
            {
                if (awakeVisble == true)
                {
                    rectTransform.DOAnchorPosX(appearPosX, 0f);
                }
                else
                {
                    rectTransform.DOAnchorPosX(disappearPosX, 0f);
                }
            }
        }
    }

    public override void Init()
    {
        
    }

    public override void Appear()
    {
        appearDelegate?.Invoke();
    }

    public override void Disappear()
    {
        disappearDelegate?.Invoke();
    }
}
