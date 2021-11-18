using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TargetMakerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI distanceText;
    private RectTransform _rectTransform;
    private Image _image;
    private Transform _target;

    public Transform Target { get => _target; set => _target = value; }

    private void Awake()
    {
        _image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (_target == null)
        {
            return;
        }
        if (Vector3.Dot((_target.position - Camera.main.transform.position).normalized, Camera.main.transform.forward) < 0)
        {
            _image.enabled = false;
            distanceText.gameObject.SetActive(false);
            return;
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(_target.position);

        if(screenPos.x < 0f || screenPos.x > Screen.currentResolution.width
            || screenPos.y < 0f || screenPos.y > Screen.currentResolution.height)
        {
            _image.enabled = false;
            distanceText.gameObject.SetActive(false);
            return;
        }
        else
        {
            distanceText.gameObject.SetActive(true);
            _image.enabled = true;
        }

        _rectTransform.transform.position = new Vector2(screenPos.x, screenPos.y);

        float distance = Vector3.Distance(_target.position, Camera.main.transform.position);
        distanceText.text = (int)distance + "M";
    }
}
