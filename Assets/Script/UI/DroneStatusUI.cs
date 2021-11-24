using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DroneStatusUI : MonoBehaviour
{
    private Canvas _canvas;

    [Header("BLine")]

    [SerializeField] private GameObject blineUiRoot;

    [SerializeField] private Image droneCenterImage;
    [SerializeField] private Image droneEyeImage;
    [SerializeField] private Image droneLineImage;

    [SerializeField] private List<Image> frontIcon = new List<Image>();
    [SerializeField] private List<Image> backIcon = new List<Image>();

    private List<float> originY = new List<float>();
    [SerializeField]private GameObject[] hpIcons = new GameObject[10];

    private int currentHp = 8;

    [Header("Final")]

    [SerializeField] private GameObject finalHpUiRoot;

    private int damageCount = 0;

    [SerializeField] private Image first_underLeft_front;
    [SerializeField] private Image first_underLeft_back;

    [SerializeField] private Image second_underRight_front;
    [SerializeField] private Image second_underRight_back;

    [SerializeField] private Image third_upLeft_front;
    [SerializeField] private Image third_upLeft_back;

    [SerializeField] private Image fourth_upRight_front;
    [SerializeField] private Image fourth_upRight_back;

    [SerializeField] private Image fifth_upLeft_front;
    [SerializeField] private Image fifth_upLeft_back;

    [SerializeField] private Image sixth_upRight_front;
    [SerializeField] private Image sixth_upRight_back;

    [SerializeField] private Image seventh_upLeft_front;
    [SerializeField] private Image seventh_upLeft_back;

    [SerializeField] private Image eighth_upRight_front;
    [SerializeField] private Image eighth_upRight_back;

    [SerializeField] private Image nineth_center_front;
    [SerializeField] private Image nineth_center_back;

    [SerializeField] private Image ten_center;

    [SerializeField] private List<Image> frontElements = new List<Image>();
    [SerializeField] private List<Image> backElements = new List<Image>();

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;

        foreach (var front in frontIcon)
        {
            originY.Add(front.rectTransform.anchoredPosition.y);
        }
    }

    public void InitDronHpUi()
    {
        droneCenterImage.DOFade(0.0f, 0.0f);
        droneLineImage.DOFade(0.0f, 0.0f);
        droneEyeImage.DOFade(0.0f, 0.0f);

        for (int i = 0; i < frontIcon.Count; i++)
        {
            frontIcon[i].DOFade(0.0f,0.0f);
            frontIcon[i].rectTransform.DOScale(new Vector3(2.5f, 2.5f, 2.5f), 0.0f);
            frontIcon[i].color = Color.white;
            frontIcon[i].rectTransform.anchoredPosition = new Vector2(frontIcon[i].rectTransform.anchoredPosition.x, originY[i]);
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
            blineUiRoot.SetActive(true);
            finalHpUiRoot.SetActive(false);
            InitDronHpUi();
            Appear();

            nineth_center_front.rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.0f);
            nineth_center_front.DOFade(1f, 0.0f);
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

    public void InitFinalHpUi()
    {
        blineUiRoot.SetActive(false);
        finalHpUiRoot.SetActive(true);

        foreach(var front in frontElements)
        {
            front.DOFillAmount(0, 0f);
            front.DOFade(1.0f, 0f);
            front.rectTransform.DOScale(new Vector3(1, 1, 1), 0f);
        }

        foreach (var back in backElements)
        {
            back.DOFade(0, 0f);
            back.color = Color.white;
        }

        damageCount = 0;

        AppearFinalUi();
    }

    public void AppearFinalUi()
    {
        first_underLeft_front.DOFillAmount(1f, 1f);
        second_underRight_front.DOFillAmount(1f, 1f);

        seventh_upLeft_front.DOFillAmount(1f, 0.6f);
        eighth_upRight_front.DOFillAmount(1f, 0.6f).OnComplete(()=> 
        {
            fifth_upLeft_front.DOFillAmount(1f, 0.6f);
            sixth_upRight_front.DOFillAmount(1f, 0.6f).OnComplete(()=>
            {
                third_upLeft_front.DOFillAmount(1f, 0.6f);
                fourth_upRight_front.DOFillAmount(1f, 0.6f);
            });
        });
    }

    public void Damage()
    {
        if(damageCount == 0)
        {
            first_underLeft_front.rectTransform.DOScale(new Vector3(2f, 2f, 2f), 0.5f);
            first_underLeft_front.DOFade(0.0f, 0.5f);
            first_underLeft_back.DOFade(1f, 0f);
            first_underLeft_back.color = Color.red;
        }
        else if(damageCount == 1)
        {
            second_underRight_front.rectTransform.DOScale(new Vector3(2f, 2f, 2f), 0.5f);
            second_underRight_front.DOFade(0.0f, 0.5f);
            second_underRight_back.DOFade(1f, 0f);
            second_underRight_back.color = Color.red;
        }
        else if (damageCount == 2)
        {
            third_upLeft_front.rectTransform.DOScale(new Vector3(2f, 2f, 2f), 0.5f);
            third_upLeft_front.DOFade(0.0f, 0.5f);
            third_upLeft_back.DOFade(1f, 0f);
            third_upLeft_back.color = Color.red;
        }
        else if (damageCount == 3)
        {
            fourth_upRight_front.rectTransform.DOScale(new Vector3(2f, 2f, 2f), 0.5f);
            fourth_upRight_front.DOFade(0.0f, 0.5f);
            fourth_upRight_back.DOFade(1f, 0f);
            fourth_upRight_back.color = Color.red;
        }
        else if (damageCount == 4)
        {
            fifth_upLeft_front.rectTransform.DOScale(new Vector3(2f, 2f, 2f), 0.5f);
            fifth_upLeft_front.DOFade(0.0f, 0.5f);
            fifth_upLeft_back.DOFade(1f, 0f);
            fifth_upLeft_back.color = Color.red;
        }
        else if (damageCount == 5)
        {
            sixth_upRight_front.rectTransform.DOScale(new Vector3(2f, 2f, 2f), 0.5f);
            sixth_upRight_front.DOFade(0.0f, 0.5f);
            sixth_upRight_back.DOFade(1f, 0f);
            sixth_upRight_back.color = Color.red;
        }
        else if (damageCount == 6)
        {
            seventh_upLeft_front.rectTransform.DOScale(new Vector3(2f, 2f, 2f), 0.5f);
            seventh_upLeft_front.DOFade(0.0f, 0.5f);
            seventh_upLeft_back.DOFade(1f, 0f);
            seventh_upLeft_back.color = Color.red;
        }
        else if (damageCount == 7)
        {
            eighth_upRight_front.rectTransform.DOScale(new Vector3(2f, 2f, 2f), 0.5f);
            eighth_upRight_front.DOFade(0.0f, 0.5f);
            eighth_upRight_back.DOFade(1f, 0f);
            eighth_upRight_back.color = Color.red;
        }
        else if (damageCount == 8)
        {
            //nineth_center_front.rectTransform.DOScale(new Vector3(2f, 2f, 2f), 0.5f);
            nineth_center_front.DOFade(0.0f, 0.5f);
            nineth_center_back.DOFade(1f, 0f);
            nineth_center_back.color = Color.red;
        }
        else if (damageCount == 9)
        {

        }
        else if (damageCount == 10)
        {

        }
        damageCount++;
    }


    private void Update()
    {
        //if (Keyboard.current.digit6Key.wasPressedThisFrame)
        //{
        //    Enable(true);
        //}
        //if (Keyboard.current.digit7Key.wasPressedThisFrame)
        //{
        //    InitFinalHpUi();
        //}
        //if (Keyboard.current.digit8Key.wasPressedThisFrame)
        //{
        //    Damage();
        //}
    }
}
