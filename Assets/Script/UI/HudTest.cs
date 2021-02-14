using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class HudTest : MonoBehaviour
{
    public ButtonHUD button1;
    public ButtonHUD button2;
    public ButtonHUD button3;

    void Start()
    {
      
    }

    void Update()
    {
        
    }


    public void HUDActive()
    {
        button3.Appear(0.2f, () => button2.Appear(0.2f, () => button1.Appear(0.2f,()=>GameManager.Instance.SwitchMenuDone())));

    }

    public void HUDDissable()
    {
        button1.Disappear(0.2f, () => button2.Disappear(0.2f, () => button3.Disappear(0.2f, () => GameManager.Instance.SwitchMenuDone())));

    }

    public void HUDDissable(Action action)
    {
        button1.Disappear(0.2f, () => button2.Disappear(0.2f, () => button3.Disappear(0.2f, () => { GameManager.Instance.SwitchMenuDone(); action(); })));

    }
}
