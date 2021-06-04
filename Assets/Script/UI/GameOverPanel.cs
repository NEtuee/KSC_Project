using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : EscMenu
{
    private Canvas _canvas;
    public ImageBaseButton restartButton;
    public ImageBaseButton titleButton;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
        _canvas.sortingOrder = 0;
        restartButton.Interactable = false;
        titleButton.Interactable = false;
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

            restartButton.Interactable = true;
            titleButton.Interactable = true;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;            
        }
        else
        {
            _canvas.sortingOrder = 0;
            _canvas.enabled = false;
            
            restartButton.Interactable = false;
            titleButton.Interactable = false;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            restartButton.Select(false);
            titleButton.Select(false);
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

    public override void Init()
    {
        throw new NotImplementedException();
    }
}
