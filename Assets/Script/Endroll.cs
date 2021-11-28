using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.SceneManagement;


public class Endroll : MonoBehaviour
{
    public InputAction skipKey;

    [SerializeField] private RectTransform root;

    [SerializeField] private Image fade;

    private bool skip = false;
    void Start()
    {
        skipKey.performed += _ => OnSkip();

        root.DOAnchorPosY(555f, 60f).SetDelay(3f).OnComplete(() =>
        {
            skip = true;
            fade.DOFade(1.0f, 3f).OnComplete(()=>
            {
                LoadTitleScene();
            });
        });
    }
    private void OnSkip()
    {
        if (skip == true)
            return;

        skip = true;
        fade.DOFade(1.0f, 3f).OnComplete(()=>
        {
            LoadTitleScene();
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


    public void LoadTitleScene()
    {
        SceneManager.LoadScene("MainTitle_NewStucture_latest");
    }
}
