using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DroneStatusUI : MonoBehaviour
{
    private Canvas _canvas;

    [SerializeField] private Image droneCenterImage;
    [SerializeField] private Image droneEyeImage;
    [SerializeField] private Image droneLineImage;

    [SerializeField] private List<Image> frontIcon = new List<Image>();
    [SerializeField] private List<Image> backIcon = new List<Image>();

    private List<float> originY = new List<float>();
    [SerializeField]private GameObject[] hpIcons = new GameObject[10];

    private int currentHp = 8;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;
    }

    public void InitDronHpUi()
    {
        droneCenterImage.DOFade(0.0f, 0.0f);
        droneLineImage.DOFade(0.0f, 0.0f);
        droneEyeImage.DOFade(0.0f, 0.0f);

        foreach (var front in frontIcon)
        {
            front.DOFade(0.0f,0.0f);
            front.rectTransform.DOScale(new Vector3(2.5f, 2.5f, 2.5f), 0.0f);
            front.color = Color.white;
            originY.Add(front.rectTransform.anchoredPosition.y);
        }

        foreach(var back in backIcon)
        {
            back.enabled = false;
        }

        currentHp = 8;
    }

    public void SetHpCount(int count)
    {
        count = Mathf.Clamp(count , 0, 8);
        for(int i = count+1; i <= currentHp; i++)
        {
            backIcon[i - 1].enabled = true;
            frontIcon[i - 1].rectTransform.DOAnchorPosY(frontIcon[i - 1].rectTransform.anchoredPosition.y + 50f, 0.5f);
            frontIcon[i - 1].DOFade(0f, 0.5f);
            frontIcon[i - 1].color = Color.red;
        }
        currentHp = count;
    }

    public void Enable(bool enable)
    {
        _canvas.enabled = enable;

        if (enable)
        {
            InitDronHpUi();
            Appear();
        }
    }

    public void Appear()
    {
        StartCoroutine(AppearCoroutine());
    }

    IEnumerator AppearCoroutine()
    {
        droneCenterImage.DOFade(1f, 0.5f);
        droneLineImage.DOFade(1f, 0.5f);
        droneEyeImage.DOFade(1f, 0.5f);

        yield return new WaitForSeconds(0.6f);

        for (int i = 0; i < frontIcon.Count; i++)
        {
            frontIcon[i].rectTransform.DOScale(new Vector3(1, 1, 1), 0.3f);
            frontIcon[i].DOFade(1f, 0.3f);
            yield return new WaitForSeconds(0.15f);
        }
    }

    //private void Update()
    //{
    //    if(Keyboard.current.bKey.wasPressedThisFrame)
    //    {
    //        Enable(true);
    //    }
    //    if (Keyboard.current.digit7Key.wasPressedThisFrame)
    //    {
    //        SetHpCount(7);
    //    }
    //    if (Keyboard.current.digit6Key.wasPressedThisFrame)
    //    {
    //        SetHpCount(6);
    //    }
    //}
}
