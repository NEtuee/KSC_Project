using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Triggers;
using UniRx;

/// <summary>
/// 게이지바를 표시하는 UI 입니다.
/// 단순히 외부에서 값을 받아와서 자기가 가진 Image의 fillAmount값을 갱신합니다.
/// </summary>
public class GageBarUI : MonoBehaviour
{
    public enum GageUpdateType
    {
        Direct,Lerp,SmoothStep
    }

    [SerializeField] private Image gageImage;
    [SerializeField] private GageUpdateType updateType;
    [SerializeField] private float updateSpeed = 4f;

    [SerializeField]private float updateValue;

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

        switch(updateType)
        {
            case GageUpdateType.Direct:
                {
                    this.UpdateAsObservable()
                        .Subscribe(_ => gageImage.fillAmount = updateValue);
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
        }
    }

    public void SetValue(float value)
    {
        updateValue = value;
    }
}
