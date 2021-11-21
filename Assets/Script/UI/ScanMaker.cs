using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanMaker : MonoBehaviour
{
    public float reductionSize = 25f;
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

        Vector3 screenPos = Vector3.zero;
        while (curTime < time)
        {
            if(collider == null || !collider.gameObject.activeInHierarchy)
            {
                break;
            }
            if(Vector3.Dot((collider.bounds.center - Camera.main.transform.position).normalized, Camera.main.transform.forward) > 0)
                 screenPos = Camera.main.WorldToScreenPoint(collider.bounds.center);
            Vector3 minPos = Camera.main.WorldToScreenPoint(collider.bounds.min);
            Vector3 maxPos = Camera.main.WorldToScreenPoint(collider.bounds.max);

            float size = 0.0f;
            if (screenPos.x <= 0.0f || screenPos.x >= Screen.currentResolution.width ||
                screenPos.y <= 0.0f || screenPos.y >= Screen.currentResolution.height)
            {
                size = reductionSize;
            }
            else
            {
                float height = Mathf.Abs(maxPos.y - minPos.y);
                float width = Mathf.Abs(maxPos.x - minPos.x);
                size = height > width ? height : width;
                size = Mathf.Clamp(size, 50.0f, 150.0f);
            }

            screenPos.x = Mathf.Clamp(screenPos.x, 0f, Screen.currentResolution.width);
            screenPos.y = Mathf.Clamp(screenPos.y, 0f, Screen.currentResolution.height);
            //Debug.Log(Vector3.Dot((collider.bounds.center - Camera.main.transform.position).normalized,Camera.main.transform.forward));
            _rectTransform.transform.position = new Vector2(screenPos.x, screenPos.y);
            _rectTransform.sizeDelta = new Vector2(size, size);

            curTime += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        _visible = false;
    }
}
