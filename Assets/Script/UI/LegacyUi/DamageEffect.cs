using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DamageEffect : MonoBehaviour
{
    public Image hitEffectImage;
    public void Effect()
    {
        hitEffectImage.DOKill();

        hitEffectImage.DOFade(1.0f, 0.1f).OnComplete(()=>hitEffectImage.DOFade(0.0f, 0.5f));
    }
}
