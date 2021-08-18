using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameTutorialCtrl : MonoBehaviour
{
    [SerializeField] private MenuPage tutorialPage;
    [SerializeField] private BaseAppearButton climbingTapButton;
    [SerializeField] private BaseAppearButton moveTapButton;
    [SerializeField] private BaseAppearButton specialTapButton;
    [SerializeField] private BaseAppearButton scanEmpTapButton;
    [SerializeField] private BaseAppearButton empTapButton;
    [SerializeField] private Canvas videoCanvas;
    [SerializeField] private Canvas keyGuideCanvas;
    [SerializeField] private GameObject layoutImage;
    [SerializeField] private MenuPage earlyTutorialButtonPage;
    [SerializeField] private MenuPage noticeTutorialPage;

    [SerializeField] private MenuPage[] tutorialElements;
    
    public void Active(InGameTutorialType type)
    {
        tutorialPage.Active(true);
        
        switch(type)
        {
            case InGameTutorialType.Climbing:
                climbingTapButton.onClick.Invoke();
                moveTapButton.gameObject.SetActive(false);
                specialTapButton.gameObject.SetActive(false);
                scanEmpTapButton.gameObject.SetActive(false);
                empTapButton.gameObject.SetActive(false);
                break;
            case InGameTutorialType.Move:
                climbingTapButton.gameObject.SetActive(false);
                moveTapButton.onClick.Invoke();
                specialTapButton.gameObject.SetActive(false);
                scanEmpTapButton.gameObject.SetActive(false);
                empTapButton.gameObject.SetActive(false);
                break;
            case InGameTutorialType.Special:
                climbingTapButton.gameObject.SetActive(false);
                moveTapButton.gameObject.SetActive(false);
                specialTapButton.onClick.Invoke();
                scanEmpTapButton.gameObject.SetActive(false);
                empTapButton.gameObject.SetActive(false);
                break;
            case InGameTutorialType.Scan:
                climbingTapButton.gameObject.SetActive(false);
                moveTapButton.gameObject.SetActive(false);
                specialTapButton.gameObject.SetActive(false);
                scanEmpTapButton.onClick.Invoke();
                empTapButton.gameObject.SetActive(false);
                break;
            case InGameTutorialType.Emp:
                climbingTapButton.gameObject.SetActive(false);
                moveTapButton.gameObject.SetActive(false);
                specialTapButton.gameObject.SetActive(false);
                scanEmpTapButton.gameObject.SetActive(false);
                empTapButton.onClick.Invoke();
                break;
            case InGameTutorialType.Key:
                climbingTapButton.gameObject.SetActive(false);
                moveTapButton.gameObject.SetActive(false);
                specialTapButton.gameObject.SetActive(false);
                scanEmpTapButton.gameObject.SetActive(false);
                empTapButton.gameObject.SetActive(false);
                videoCanvas.enabled = false;
                keyGuideCanvas.enabled = true;
                layoutImage.SetActive(false);

                foreach(var page in tutorialElements)
                {
                    page.Active(false);
                }
                break;
            case InGameTutorialType.Ealry:
                climbingTapButton.gameObject.SetActive(false);
                moveTapButton.gameObject.SetActive(false);
                specialTapButton.gameObject.SetActive(false);
                scanEmpTapButton.gameObject.SetActive(false);
                empTapButton.gameObject.SetActive(false);
                earlyTutorialButtonPage.Active(true);
                foreach (var page in tutorialElements)
                {
                    page.Active(false);
                }
                break;
            case InGameTutorialType.Notice:
                climbingTapButton.gameObject.SetActive(false);
                moveTapButton.gameObject.SetActive(false);
                specialTapButton.gameObject.SetActive(false);
                scanEmpTapButton.gameObject.SetActive(false);
                empTapButton.gameObject.SetActive(false);
                videoCanvas.enabled = false;
                noticeTutorialPage.Active(true);
                foreach (var page in tutorialElements)
                {
                    page.Active(false);
                }
                break;
        }
    }

    public void Disable()
    {
        climbingTapButton.gameObject.SetActive(true);
        moveTapButton.gameObject.SetActive(true);
        specialTapButton.gameObject.SetActive(true);
        scanEmpTapButton.gameObject.SetActive(true);
        empTapButton.gameObject.SetActive(true);
        videoCanvas.enabled = true;
        keyGuideCanvas.enabled = false;
        layoutImage.SetActive(true);
        earlyTutorialButtonPage.Active(false);
        noticeTutorialPage.Active(false);

        tutorialPage.Active(false);
    }

    public enum InGameTutorialType
    {
        Climbing,Move,Special,Scan,Emp,Key,Ealry,Notice
    }
}

namespace MD
{
    public class InGameTutorialTypeData : MessageData
    {
        public InGameTutorialCtrl.InGameTutorialType type;
    }
}
