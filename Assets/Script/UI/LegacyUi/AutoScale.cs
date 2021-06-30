using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScale : MonoBehaviour
{
    private RectTransform rect;

    [SerializeField] private Vector3 standardScale;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        if(rect == null || GameManager.Instance == null)
        {
            return;
        }

        float camDist = GameManager.Instance.cameraManager.GetCameraDistance();
        rect.localScale = standardScale * camDist;
    }
}
