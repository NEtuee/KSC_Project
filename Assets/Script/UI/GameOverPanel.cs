using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : EscMenu
{
    private Canvas _canvas;
    public MainTitleButton restartButton;
    public MainTitleButton titleButton;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        _canvas.sortingOrder = 0;
    }

    public void OnRestartButton()
    {
        Active(false);
        GameManager.Instance.RestartLevel();
    }

    public void OnTitleButton()
    {
        GameManager.Instance.LoadTitleScene();
    }
    
    public override void Active(bool active)
    {
        if (active == true)
        {
            _canvas.sortingOrder = 5;
            _canvas.enabled = true;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            restartButton.Appear();
        }
        else
        {
            _canvas.sortingOrder = 0;
            _canvas.enabled = false;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            restartButton.Init();
            titleButton.Init();
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
