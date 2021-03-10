using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Triggers;
using UniRx;
using DG.Tweening;
using UnityEngine.Events;
/// <summary>
/// 게이지바를 표시하는 UI 입니다.
/// 단순히 외부에서 값을 받아와서 자기가 가진 Image의 fillAmount값을 갱신합니다.
/// </summary>
public class GageBarUI : MonoBehaviour
{
    public enum GageUpdateType
    {
        Direct,Lerp,SmoothStep,MoveToward
    }

    [SerializeField] private Image gageImage;
    [SerializeField] private GageUpdateType updateType;
    [SerializeField] private float updateSpeed = 4f;

    [SerializeField]private float updateValue;
    [SerializeField] private float displayTime = 5f;
    [SerializeField] private bool isDisplay = false;
    [SerializeField] private float displayAlpha = 0.7f;
    [SerializeField] private bool visible = false;
    private float currentDisplayTime = 0.0f;

    public UnityEvent appearChain;
    public UnityEvent disappearChain;

    void Start()
    {
        if(gageImage == null)
        {
            gageImage = GetComponent<Image>();

            if(gageImage == null)
            {
                Debug.LogError("This GameObject is Not Image");
            }
        }

        switch (updateType)
        {
            case GageUpdateType.Direct:
                {
                    this.UpdateAsObservable()
                        .Subscribe(_ => {

                            gageImage.fillAmount = updateValue;
                            currentDisplayTime -= Time.deltaTime;
                            if (currentDisplayTime <= 0.0f) currentDisplayTime = 0.0f;

                            if (visible)
                            {
                                return;
                            }

                            if (currentDisplayTime <= 0.0f && isDisplay == true)
                            {
                                isDisplay = false;
                                gageImage.DOFade(0.0f, 1.0f);
                                disappearChain?.Invoke();
                            }
                            
                        });
                }
                break;
            case GageUpdateType.Lerp:
                {
                    this.UpdateAsObservable()
                       .Subscribe(_ => { gageImage.fillAmount = Mathf.Lerp(gageImage.fillAmount, updateValue, updateSpeed * Time.deltaTime);});
                }
                break;
            case GageUpdateType.SmoothStep:
                {
                    this.UpdateAsObservable()
                      .Subscribe(_ => gageImage.fillAmount = Mathf.SmoothStep(gageImage.fillAmount, updateValue, updateSpeed * Time.deltaTime));
                }
                break;
            case GageUpdateType.MoveToward:
                {
                    this.UpdateAsObservable()
                      .Subscribe(_ => 
                      {
                          gageImage.fillAmount = Mathf.MoveTowards(gageImage.fillAmount, updateValue, updateSpeed * Time.deltaTime);
                          currentDisplayTime -= Time.deltaTime;
                          if (currentDisplayTime <= 0.0f) currentDisplayTime = 0.0f;

                          if (visible)
                          {
                              return;
                          }

                          if(currentDisplayTime <= 0.0f && isDisplay == true)
                          {
                              isDisplay = false;
                              gageImage.DOFade(0.0f, 1.0f);
                              disappearChain?.Invoke();
                          }
                         
                      });
                }
                break;
        }
    }

    public void SetValue(float value)
    {
        currentDisplayTime = displayTime;
        if (isDisplay == false)
        {
            isDisplay = true;
            if (visible == false)
            {
                gageImage.DOFade(displayAlpha, 0.5f);
                appearChain?.Invoke();
            }
        }

        updateValue = value;
    }

    public void Visible(bool result)
    {
        visible = result;
        if (result == true)
        {
            gageImage.DOKill();
            gageImage.DOFade(displayAlpha, 0.5f);
            appearChain?.Invoke();
        }
        else
        {
            isDisplay = true;
            currentDisplayTime = displayTime;
        }
    }
}
