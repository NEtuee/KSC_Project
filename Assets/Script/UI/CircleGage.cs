using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CircleGage : MonoBehaviour
{
    [SerializeField] private Image gage;
    
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private float targetValue;
    private float currentValue;

    private Color origincolor;
    private Color originTextColor;
    void Start()
    {
        currentValue = 0.0f;
        origincolor = gage.color;
        originTextColor = valueText.color;
        gage.fillAmount = 0.0f;
        valueText.text = "";
    }

    public void Active()
    {
        currentValue = 0.0f;
        gage.color = origincolor;
        valueText.text = "";
        StartCoroutine(Sync());
    }

    IEnumerator Sync()
    {
        while(currentValue < targetValue)
        {
            currentValue += 80f * Time.deltaTime;
            currentValue = Mathf.Clamp(currentValue, 0.0f, targetValue);

            gage.fillAmount = currentValue / 100f;

            valueText.text = ((int)currentValue).ToString();

            yield return null;
        }
    }

    public void Disapper()
    {
        valueText.DOFade(0f, 0.05f).OnComplete(()=> { gage.fillAmount = 0.0f; valueText.text = ""; valueText.color = originTextColor; gage.color = origincolor; });
        gage.DOFade(0f, 0.05f);
    }
}
