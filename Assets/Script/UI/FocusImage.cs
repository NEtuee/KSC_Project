using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusImage : MonoBehaviour
{
    public Image TargetImage { get => _targetImage; set => _targetImage = value;}

    public Sprite focusImage;
    public Sprite defocusImage;

    private Image _targetImage;

    void Start()
    {
        if(TryGetComponent<Image>(out _targetImage) == false)
        {
            Debug.Log("Not Set Image");
        }

        OnDefocus();
    }

    public void OnFocus()
    {
        _targetImage.sprite = focusImage;
    }

    public void OnDefocus()
    {
        _targetImage.sprite = defocusImage;
    }
}
