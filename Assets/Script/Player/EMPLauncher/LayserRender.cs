using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayserRender : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    private float activeTime = 0.0f;
    private bool active = false;
    private float duration;
    private float currentWidth;

    void Start()
    {
        
    }

    void Update()
    {
        if(active)
        {
            float amount = 1f-Mathf.InverseLerp(activeTime, activeTime + duration, Time.time);
            lineRenderer.startWidth = currentWidth * amount;
            lineRenderer.endWidth = currentWidth * amount;

            if(amount <= Mathf.Epsilon)
            {
                active = false;
            }
        }
    }

    public void Active(Vector3 start, Vector3 end, float time, float width)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        currentWidth = width;
        duration = time;
        activeTime = Time.time;
        active = true;
    }
}
