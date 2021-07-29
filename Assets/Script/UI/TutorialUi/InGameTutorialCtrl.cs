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
        }
    }

    public void Disable()
    {
        climbingTapButton.gameObject.SetActive(true);
        moveTapButton.gameObject.SetActive(true);
        specialTapButton.gameObject.SetActive(true);
        scanEmpTapButton.gameObject.SetActive(true);
        empTapButton.gameObject.SetActive(true);

        tutorialPage.Active(false);
    }

    public enum InGameTutorialType
    {
        Climbing,Move,Special,Scan,Emp
    }
}
