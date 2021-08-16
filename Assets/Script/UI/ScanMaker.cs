using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanMaker : MonoBehaviour
{
    private RectTransform _rectTransform;
    private bool _visible;
    public bool Visible { get => _visible; }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void Active(Vector3 center, Vector3 min, Vector3 max, float time = 4f)
    {
        StartCoroutine(Focus(center, min, max,time));
    }

    IEnumerator Focus(Vector3 center, Vector3 min, Vector3 max, float time)
    {
        float curTime = 0f;
        _visible = true;
        while (curTime < time)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(center);
            Vector3 minPos = Camera.main.WorldToScreenPoint(min);
            Vector3 maxPos = Camera.main.WorldToScreenPoint(max);

            float height = Mathf.Sign(maxPos.y - minPos.y);
            float width = Mathf.Sign(maxPos.x - minPos.x);
            float size = height > width ? height : width;

            _rectTransform.anchoredPosition = new Vector2(screenPos.x, screenPos.y);
            _rectTransform.sizeDelta = new Vector2(size, size);

            yield return new WaitForFixedUpdate();
        }
        _visible = false;
    }
}
