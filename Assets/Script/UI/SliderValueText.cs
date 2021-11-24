using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValueText : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;


    public void UpdataValue()
    {
        text.text = ((int)(slider.value * 100f)).ToString();
    }
}
