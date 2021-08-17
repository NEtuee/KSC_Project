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

    public void Active(/*Transform center, Vector3 min, Vector3 max,*/Collider collider, float time = 4f)
    {
        StartCoroutine(Focus(collider,time));//collider.bounds.center, collider.bounds.min, collider.bounds.max,time));
    }

    IEnumerator Focus(/*Vector3 center, Vector3 min, Vector3 max,*/ Collider collider, float time)
    {
        float curTime = 0f;
        _visible = true;
        while (curTime < time)
        {
            if(collider == null)
            {
                break;
            }
            Vector3 screenPos = Camera.main.WorldToScreenPoint(collider.bounds.center);
            Vector3 minPos = Camera.main.WorldToScreenPoint(collider.bounds.min);
            Vector3 maxPos = Camera.main.WorldToScreenPoint(collider.bounds.max);

            float height = Mathf.Abs(maxPos.y - minPos.y);
            float width = Mathf.Abs(maxPos.x - minPos.x);
            float size = height > width ? height : width;
            size = Mathf.Clamp(size, 50.0f, 150.0f);

            _rectTransform.transform.position = new Vector2(screenPos.x, screenPos.y);
            _rectTransform.sizeDelta = new Vector2(size, size);

            curTime += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        _visible = false;
    }
}
