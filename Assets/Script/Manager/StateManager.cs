using System;
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
    public SkinnedMeshRenderer bagRenderer;
    public Material _bagMat;
    void Start()
    {
        _bagMat = bagRenderer.sharedMaterial;
        
        ((PlayerCtrl_Ver2)(GameManager.Instance.player)).hpPackCount.Subscribe(value =>
        {
            if (((PlayerCtrl_Ver2)(GameManager.Instance.player)).GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
                hpPackUI.SetValue(value, false);
            else
            {
                hpPackUI.SetValue(value);
                hpUI.SetVisible(true);
                staminaUI.SetVisible(true);
                energyUI.SetVisible(true);
            }
        });

        GameManager.Instance.player.hp.Subscribe(value =>
        {
            _bagMat.SetFloat("Vector1_5338de784f7d4439aba250082f9a53e3", value*0.01f);
            if (((PlayerCtrl_Ver2)(GameManager.Instance.player)).GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
                hpUI.SetValue(value, false);
            else
            {
                hpUI.SetValue(value);
                hpPackUI.SetVisible(true);
                staminaUI.SetVisible(true);
                energyUI.SetVisible(true);
            }
        });

        GameManager.Instance.player.stamina.Subscribe(value =>
        {
            if (((PlayerCtrl_Ver2)(GameManager.Instance.player)).GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
                staminaUI.SetValue(value,false);
            else
            {
                staminaUI.SetValue(value);
                hpUI.SetVisible(true);
                energyUI.SetVisible(true);
                hpPackUI.SetVisible(true);
            }
        });

        GameManager.Instance.player.energy.Subscribe(value =>
        {
            if (((PlayerCtrl_Ver2)(GameManager.Instance.player)).GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
                energyUI.SetValue(value,false);
            else
            {
                energyUI.SetValue(value);
                hpUI.SetVisible(true);
                staminaUI.SetVisible(true);
                hpPackUI.SetVisible(true);
            }
        });
    }

    public void Visible(bool result)
    {
        hpUI.SetVisible(result);
        staminaUI.SetVisible(result);
        energyUI.SetVisible(result);
    }

    private void FixedUpdate()
    {
        //Debug.Log(bagRenderer.sharedMaterial.GetFloat("Vector1_5338de784f7d4439aba250082f9a53e3"));
    }
}
