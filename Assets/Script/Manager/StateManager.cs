using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class StateManager : MonoBehaviour
{
    public FadeUI hpUI;
    public FadeUI staminaUI;
    public FadeUI energyUI;
    public HpPackUI hpPackUI;
    void Start()
    {
        ((PlayerCtrl_Ver2)(GameManager.Instance.player)).hpPackCount.Subscribe(value =>
        {
            if (((PlayerCtrl_Ver2)(GameManager.Instance.player)).GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
                hpPackUI.SetValue(value, false);
            else
            {
                hpPackUI.SetValue(value);
                hpUI.SetVisible(true);
            }
        });

        GameManager.Instance.player.hp.Subscribe(value =>
        {
            if (((PlayerCtrl_Ver2)(GameManager.Instance.player)).GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
                hpUI.SetValue(value, false);
            else
            {
                hpUI.SetValue(value);
                hpPackUI.SetVisible(true);
            }
        });

        GameManager.Instance.player.stamina.Subscribe(value =>
        {
            if (((PlayerCtrl_Ver2)(GameManager.Instance.player)).GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
                staminaUI.SetValue(value,false);
            else
                staminaUI.SetValue(value);
        });

        GameManager.Instance.player.energy.Subscribe(value =>
        {
            if (((PlayerCtrl_Ver2)(GameManager.Instance.player)).GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
                energyUI.SetValue(value,false);
            else
                energyUI.SetValue(value);
        });
    }

    public void Visible(bool result)
    {
        hpUI.SetVisible(result);
        staminaUI.SetVisible(result);
        energyUI.SetVisible(result);
    }
}
