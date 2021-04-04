using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class StateManager : MonoBehaviour
{
    public GageBarUI hpUI;
    public GageBarUI staminaUI;
    public GageBarUI energyUI;
    void Start()
    {
        GameManager.Instance.player.hp.Subscribe(value =>
        {
            hpUI.SetValue(value);
        });

        GameManager.Instance.player.stamina.Subscribe(value =>
        {
            staminaUI.SetValue(value);
        });

        GameManager.Instance.player.energy.Subscribe(value =>
        {
            energyUI.SetValue(value);
        });
    }

    public void Visible(bool result)
    {
        hpUI.Visible(result);
        staminaUI.Visible(result);
        energyUI.Visible(result);
    }
}
