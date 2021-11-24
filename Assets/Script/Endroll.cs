using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

public class Endroll : MonoBehaviour
{
    public InputAction skipKey;

    [SerializeField] private RectTransform root;

    [SerializeField] private Image fade;

    private bool skip = false;
    void Start()
    {
        skipKey.performed += _ => OnSkip();

        root.DOAnchorPosY(555f, 60f).OnComplete(() =>
        {
            skip = true;
            fade.DOFade(1.0f, 3f).OnComplete(()=>
            { 
            });
        });
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    private void OnSkip()
    {
        if (skip == true)
            return;

        skip = true;
        fade.DOFade(1.0f, 3f).OnComplete(()=>
        { 
        });
    }

    private void OnEnable()
    {
        skipKey.Enable();
    }

    private void OnDisable()
    {
        skipKey.Disable();
    }
}
