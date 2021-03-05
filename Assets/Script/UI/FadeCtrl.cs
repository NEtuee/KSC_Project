using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class FadeCtrl : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI text;
    public void Appear()
    {
        text.DOFade(0.8f,0.5f);
    }

    public void Dissapear()
    {
        text.DOFade(0.0f, 1f);

    }
}
