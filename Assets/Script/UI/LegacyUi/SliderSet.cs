using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderSet : MonoBehaviour
{
    public ImageBaseButton button;
    public Slider slider;

    public void Awake()
    {
        button.Init();
        button.Interactable = false;
    }

    public void Active(bool active)
    {
        slider.interactable = active;
    }
}
