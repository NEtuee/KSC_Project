using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class TextBaseButtonUi : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
     [SerializeField] private bool interactable = true;
     [SerializeField] private bool selected = false;
     
     public TextMeshProUGUI baseText;
     public Image backGroundImage;
     [SerializeField] private Color appearColor;
     [SerializeField] private Color disappearColor;
     [SerializeField] private Color textDefaultColor;
     [SerializeField] private Color textSelectedColor;
     [SerializeField] private Color imageDefaultColor;
     [SerializeField] private Color imageSelectedColor;

     public UnityEvent onSelect;
     public UnityEvent onRelease;
     public UnityEvent onClick;

     public bool Interactable { get => interactable; set => interactable = value; }
     public bool Selected
     {
          get => selected;
          set
          {
            Select(value);
          }
     }
     
     public void Init()
     {
          if (baseText == null)
          {
               Debug.LogError("Not Set Text");
               return;
          }

          interactable = true;
          baseText.color = textDefaultColor;
          if (backGroundImage == null)
               return;
          backGroundImage.color = imageDefaultColor;
     }
     
     public void OnPointerEnter(PointerEventData eventData)
     {
          if (interactable == false)
               return;

          baseText.color = textSelectedColor;
          if (backGroundImage == null)
               return;
          backGroundImage.color = imageSelectedColor;

        GameManager.Instance.soundManager.Play(3000, Vector3.zero);
        GameManager.Instance.soundManager.SetParam(3000, 30001, 0);
    }

     public void OnPointerExit(PointerEventData eventData)
     {
          if (interactable == false || selected == true)
               return;
          
          baseText.color = textDefaultColor;
          if (backGroundImage == null)
               return;
          backGroundImage.color = imageDefaultColor;
     }

     public void OnPointerClick(PointerEventData pointerEventData)
     {
          if (interactable == false)
               return;
          
          Select(true);

               GameManager.Instance.soundManager.Play(3000, Vector3.zero);
        GameManager.Instance.soundManager.SetParam(3000, 30001, 1);
     }

     public void Appear(float duration, TweenCallback endCallback = null)
     {
          interactable = true;
          baseText.DOColor(appearColor, duration).OnComplete(endCallback);
     }
     
     public void Disappear(float duration, TweenCallback endCallback = null)
     {
          interactable = false;
          baseText.DOColor(disappearColor, duration).OnComplete(endCallback);
     }

     public void Active(bool active)
     {
          interactable = active;
          if (active)
          {
               baseText.color = textDefaultColor;
               if (backGroundImage == null)
                    return;
               backGroundImage.color = imageDefaultColor;
          }
          else
          {
               baseText.DOFade(0.0f,0.0f);
               if (backGroundImage == null)
                    return;
               backGroundImage.DOFade(0.0f,0.0f);
          }
     }

     public void Select(bool value)
     {
        if (value == selected)
            return;

          selected = value;
          if (value)
          {
               onSelect?.Invoke();
               onClick?.Invoke();

               baseText.color = textSelectedColor;
               if (backGroundImage == null)
                    return;
               backGroundImage.color = imageSelectedColor;
          }
          else
          {
               onRelease?.Invoke();
               baseText.color = textDefaultColor;
               if (backGroundImage == null)
                    return;
               backGroundImage.color = imageDefaultColor;
          }
     }
}
