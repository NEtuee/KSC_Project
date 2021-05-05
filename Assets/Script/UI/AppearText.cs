using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UniRx;

public class AppearText : MonoBehaviour
{
    private TextMeshProUGUI text;


    private FloatReactiveProperty currentCharacterSpacing = new FloatReactiveProperty();

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        text.characterSpacing = -70f;

        currentCharacterSpacing.Value = -70f;

        DOTween.To(()=>currentCharacterSpacing.Value, x => { currentCharacterSpacing.Value = x;
            text.characterSpacing = currentCharacterSpacing.Value; }, 50f, 2f).SetEase(Ease.OutQuint);
    }

    void Update()
    {
        
    }
}
