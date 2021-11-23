using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChageSprite : MonoBehaviour
{
    [SerializeField] private Sprite first;
    [SerializeField] private Sprite second;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void Change()
    {
        if (_image.sprite == first)
            _image.sprite = second;
        else
            _image.sprite = first;
    }
}
