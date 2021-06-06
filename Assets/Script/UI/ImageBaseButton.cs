using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ImageBaseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private bool interactable = true;
    [SerializeField] private bool selected = false;

    public Canvas canvas;
    public Image baseImage;
    [SerializeField] private Sprite defaultImage;
    [SerializeField] private Sprite selectedImage;

    public UnityEvent onSelect;
    public UnityEvent onRelease;
    public UnityEvent onClick;
    public UnityEvent onEnter;
    public UnityEvent onExit;

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
        if(baseImage == null)
        {
            Debug.LogError("Not Set Image Component");
            return;
        }

        interactable = true;
        baseImage.sprite = defaultImage;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (interactable == false)
            return;

        baseImage.sprite = selectedImage;
        onEnter?.Invoke();

        GameManager.Instance.soundManager.Play(3000, Vector3.zero);
        GameManager.Instance.soundManager.SetParam(3000, 30001, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (interactable == false || selected == true)
            return;

        baseImage.sprite = defaultImage;
        onExit?.Invoke();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (interactable == false)
            return;

        Select(true);

        GameManager.Instance.soundManager.Play(3000, Vector3.zero);
        GameManager.Instance.soundManager.SetParam(3000, 30001, 1);
    }

    public void Active(bool active)
    {
        interactable = active;
        if (active)
        {
            if(canvas != null)
                canvas.enabled = true;
            baseImage.sprite = defaultImage;
        }
        else
        {
            if (canvas != null)
                canvas.enabled = false;
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

            baseImage.sprite = selectedImage;
        }
        else
        {
            onRelease?.Invoke();

            baseImage.sprite = defaultImage;
        }
    }
}
