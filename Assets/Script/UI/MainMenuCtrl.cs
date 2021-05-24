using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class MainMenuCtrl : MonoBehaviour
{
    public Canvas mainTitleCanvas;
    public Canvas fadePanel;
    public Image fadeImage;

    public TitleLogoCtrl title;
    public MainTitleButton startButton;
    public MainTitleButton optionButton;
    public MainTitleButton exitButton;

    private void Start()
    {
        title.Init();
        fadeImage.color = Color.black;
        FadeIn(()=> { title.Appear(); });
    }

    private void Update()
    {
        if (InputManager.Instance.GetInput(KeybindingActions.Option) && GameManager.Instance.optionMenuCtrl.CurrentMenuState != OptionMenuCtrl.MenuType.None)
        {
            GameManager.Instance.optionMenuCtrl.InputEsc();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void LoadPlayerScene()
    {
        FadeOut(() =>
        {
            SceneManager.LoadScene("PlayerScene_Character0426");
        });
    }

    private void FadeIn(TweenCallback complete)
    {
        fadeImage.DOFade(0f, 1f).OnComplete(()=> { complete?.Invoke(); fadePanel.enabled = false; });
    }

    private void FadeOut(TweenCallback complete)
    {
        fadePanel.enabled = true;
        fadeImage.DOFade(1f, 1f).OnComplete(complete);
    }

    public void OnOptionButton()
    {
        mainTitleCanvas.sortingOrder = 2;
    }

    public void OffOption()
    {
        mainTitleCanvas.sortingOrder = 4;
    }

    public void SetButtonInteractable(bool active)
    {
        startButton.interactable = active;
        optionButton.interactable = active;
        exitButton.interactable = active;
    }
}
