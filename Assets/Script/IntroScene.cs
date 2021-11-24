using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class IntroScene : MonoBehaviour
{
    public InputAction skipAction;
    [SerializeField] private Image fade;
    [SerializeField] private bool skip = false;

    private void Awake()
    {
        skipAction.performed += _ => OnSkip();
    }

    private void OnSkip()
    {
        if (skip == true)
            return;

        skip = true;
        fade.DOFade(1f, 2f).OnComplete(() =>
        {
        });
    }

    private void OnEnable()
    {
        skipAction.Enable();
    }

    private void OnDisable()
    {
        skipAction.Disable();
    }
}
