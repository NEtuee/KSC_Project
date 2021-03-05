using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayserRender : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    private float activeTime = 0.0f;
    private bool active = false;
    private float duration;
    private float fadeDuration;
    private float currentWidth;

    void Start()
    {
        
    }

    void Update()
    {
        if(active)
        {
            if(duration > 0.0f)
            {
                duration -= Time.deltaTime;
            }
            else
            {
                float amount = 1f - Mathf.InverseLerp(activeTime, activeTime + fadeDuration, Time.time);
                lineRenderer.startWidth = currentWidth * amount;
                lineRenderer.endWidth = currentWidth * amount;

                if (amount <= Mathf.Epsilon)
                {
                    active = false;
                }
            }
        }
    }

    public void Active(Vector3 start, Vector3 end, float duration, float fadetime, float width)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        currentWidth = width;
        this.duration = duration;
        fadeDuration = fadetime;
        activeTime = Time.time + duration;
        active = true;
    }
}
