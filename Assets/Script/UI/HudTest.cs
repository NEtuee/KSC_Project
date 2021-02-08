using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HudTest : MonoBehaviour
{
    public RectTransform button1;
    public Image button1Image;
    public RectTransform button2;
    public Image button2Image;
    public RectTransform button3;
    public Image button3Image;
    public Vector2 startSize;

    void Start()
    {
        button1.sizeDelta = startSize;
        button2.sizeDelta = startSize;
        button3.sizeDelta = startSize;
        Color buttonColor = button1Image.color;
        buttonColor.a = 0f;
        button1Image.color = buttonColor;
        button2Image.color = buttonColor;
        button3Image.color = buttonColor;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            button1.DOSizeDelta(new Vector2(100f, 200f), 0.5f);
            button2.DOSizeDelta(new Vector2(100f, 200f), 0.5f);
            button3.DOSizeDelta(new Vector2(100f, 200f), 0.5f);
            button1Image.DOFade(0.4f, 0.5f);
            button2Image.DOFade(0.4f, 0.5f);
            button3Image.DOFade(0.4f, 0.5f);
        }
    }

}
