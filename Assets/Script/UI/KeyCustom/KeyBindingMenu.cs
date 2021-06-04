using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindingMenu : EscMenu
{
    public Canvas canvas;

    public GameObject keyboardPanel;
    public GameObject dualShockPanel;
    public GameObject xboxPanel;

    public List<KeyCustomItem> keyboardKeycustomItems = new List<KeyCustomItem>();
    public List<KeyCustomItem> dualShockKeycustomItems = new List<KeyCustomItem>();
    public List<KeyCustomItem> xboxKeycustomItems = new List<KeyCustomItem>();

    public TextBaseButtonUi keyboardButton;
    public TextBaseButtonUi dualShockButton;
    public TextBaseButtonUi xboxButton;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Init()
    {
        InitKeyItem();
        canvas.enabled = false;

        keyboardPanel.SetActive(false);
        dualShockPanel.SetActive(false);
        xboxPanel.SetActive(false);

        keyboardButton.Active(false);
        dualShockButton.Active(false);
        xboxButton.Active(false);
    }

    private void InitKeyItem()
    {
        foreach (var item in keyboardKeycustomItems)
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

    public override void Active(bool active)
    {
        canvas.enabled = active;
        if (active)
        {
            keyboardButton.Active(true);
            dualShockButton.Active(true);
            xboxButton.Active(true);

            keyboardButton.Select(true);
        }
        else
        {
            keyboardButton.Active(false);
            dualShockButton.Active(false);
            xboxButton.Active(false);

            keyboardButton.Select(false);
            dualShockButton.Select(false);
            xboxButton.Select(false);

            keyboardPanel.SetActive(false);
            dualShockPanel.SetActive(false);
            xboxPanel.SetActive(false);

            InputManager.Instance.InitializeKeyBind_Toggle();
            InputManager.Instance.SaveKeyBinding();
        }
    }

    public override void Appear(float duration)
    {
        throw new System.NotImplementedException();
    }

    public override void Appear(float duration, TweenCallback tweenCallback)
    {
        throw new System.NotImplementedException();
    }

    public override void Disappear(float duration)
    {
        throw new System.NotImplementedException();
    }

    public override void Disappear(float duration, TweenCallback tweenCallback)
    {
        throw new System.NotImplementedException();
    }

}
