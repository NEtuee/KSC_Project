using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow3dObjectUi : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector2 offset;

    private RectTransform _rectTransform;
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        var followPosition = Camera.main.WorldToScreenPoint(followTarget.position);
        followPosition += new Vector3(offset.x, offset.y, 0f);
        _rectTransform.position = followPosition;
    }
}
