using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCustomizeMenu : EscMenu
{
    public Canvas canvas;

    public GameObject keyboardPanel;
    public GameObject dualShockPanel;
    public GameObject xboxPanel;

    public List<KeyCustomItem> keyboardKeycustomItems = new List<KeyCustomItem>();
    public List<KeyCustomItem> dualShockKeycustomItems = new List<KeyCustomItem>();
    public List<KeyCustomItem> xboxKeycustomItems = new List<KeyCustomItem>();

    void Start()
    {
        foreach(var item in keyboardKeycustomItems)
        {
            item.Initialize(InputType.Keyboard);
        }

        foreach (var item in dualShockKeycustomItems)
        {
            item.Initialize(InputType.DualShock);
        }

        foreach (var item in xboxKeycustomItems)
        {
            item.Initialize(InputType.XboxPad);
        }

        canvas.enabled = false;
    }

    void Update()
    {
        
    }

    public void OnKeyboardPanel()
    {
        keyboardPanel.SetActive(true);
        dualShockPanel.SetActive(false);
        xboxPanel.SetActive(false);

        foreach (var item in keyboardKeycustomItems)
        {
            item.Initialize(InputType.Keyboard);
        }
    }

    public void OnKeyDualShockPanel()
    {
        dualShockPanel.SetActive(true);
        keyboardPanel.SetActive(false);
        xboxPanel.SetActive(false);

        foreach (var item in dualShockKeycustomItems)
        {
            item.Initialize(InputType.DualShock);
        }
    }

    public void OnKeyXboxPanel()
    {
        xboxPanel.SetActive(true);
        keyboardPanel.SetActive(false);
        dualShockPanel.SetActive(false);

        foreach (var item in xboxKeycustomItems)
        {
            item.Initialize(InputType.XboxPad);
        }

    }

    public override void Appear(float duration)
    {
        canvas.enabled = true;
    }

    public override void Appear(float duration, TweenCallback tweenCallback)
    {
        canvas.enabled = true;
    }

    public override void Disappear(float duration)
    {
        canvas.enabled = false;
        InputManager.Instance.InitializeKeyBind_Toggle();
    }

    public override void Disappear(float duration, TweenCallback tweenCallback)
    {
        canvas.enabled = false;
        InputManager.Instance.InitializeKeyBind_Toggle();
        tweenCallback.Invoke();
    }

    public override void Active(bool active)
    {
        if(active)
        {
            canvas.enabled = true;
        }
        else
        {
            canvas.enabled = false;
            InputManager.Instance.InitializeKeyBind_Toggle();
        }
    }
}
