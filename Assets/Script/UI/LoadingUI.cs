using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private Image loadingGageImage;
    [SerializeField] private TextMeshProUGUI loadingTipText;
    [SerializeField] private LoadingTextImageCtrl loadingText;


    public void Active(bool active)
    {
        loadingTipText.text = "";
        loadingGageImage.fillAmount = 0.0f;
        rootCanvas.enabled = active;
        loadingText.Active(active);
    }

    public void SetLoadingGageValue(float value)
    {
        value = Mathf.Clamp(0.0f, 1.0f, value);
        loadingGageImage.fillAmount = value;
    }

    public void SetLoadingTipText(string text)
    {
        loadingTipText.text = text;
    }
}
