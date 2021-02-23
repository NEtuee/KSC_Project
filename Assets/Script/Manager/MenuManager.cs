using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public EscMenu main;
    public EscMenu input;
    public EscMenu sound;

    public float inputBlendingTime = 0.08f;
    public float soundBlendingTime = 0.5f;

    public GameObject crossHair;

    private Stack<EscMenu> menuPopup = new Stack<EscMenu>();

    private bool isMenuBlend = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isMenuBlend == false)
        {
            InputEsc();
            crossHair.SetActive(!crossHair.activeSelf);
        }
    }

    public void InputEsc()
    {
        if(menuPopup.Count == 0)
        {
            menuPopup.Push(main);
            isMenuBlend = true;
            GameManager.Instance.player.Pause();
            GameManager.Instance.followTarget.Pause();
            GameManager.Instance.cameraManger.ActiveAimCamera(() =>menuPopup.Peek().Appear(0.2f, () => isMenuBlend = false));
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            return;
        }
        else if( menuPopup.Count == 1)
        {
            isMenuBlend = true;
            GameManager.Instance.player.Resume();
            GameManager.Instance.followTarget.Resume();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            GameManager.Instance.cameraManger.ActivePlayerFollowCamera(() => menuPopup.Pop().Disappear(0.2f, () => 
            { 
                isMenuBlend = false;
                GameManager.Instance.player.Resume();
                GameManager.Instance.followTarget.Resume();
            }));

            return;
        }

        isMenuBlend = true;
        if(menuPopup.Peek() == input)
        {
            menuPopup.Pop().Disappear(inputBlendingTime, () => {
                menuPopup.Peek().Appear(0.2f, () => isMenuBlend = false);
            });
        }
        else if(menuPopup.Peek() == sound)
        {
            menuPopup.Pop().Disappear(soundBlendingTime, () => {
                menuPopup.Peek().Appear(0.2f, () => isMenuBlend = false);
            });
        }
    }

    public void OnButtonInput()
    {
        isMenuBlend = true;
        menuPopup.Peek().Disappear(0.2f, () => 
        {
            menuPopup.Push(input);
            menuPopup.Peek().Appear(inputBlendingTime, () => isMenuBlend = false);
        });
    }

    public void OnButtonSound()
    {
        isMenuBlend = true;
        menuPopup.Peek().Disappear(0.2f, () =>
         {
             menuPopup.Push(sound);
             menuPopup.Peek().Appear(soundBlendingTime, () => isMenuBlend = false);
         });
    }
}
