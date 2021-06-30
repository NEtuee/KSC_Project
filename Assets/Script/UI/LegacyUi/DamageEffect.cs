using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DamageEffect : MonoBehaviour
{
    public Image hitEffectImage;

    private void Start()
    {
        ((PlayerCtrl_Ver2)GameManager.Instance.player).whenTakeDamage += HitEffect;
    }

    public void HitEffect()
    {
        hitEffectImage.DOKill();

        hitEffectImage.DOFade(1.0f, 0.1f).OnComplete(()=>hitEffectImage.DOFade(0.0f, 0.5f));
    }
}
