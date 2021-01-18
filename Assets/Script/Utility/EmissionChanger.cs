using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionChanger : MonoBehaviour
{
    [SerializeField]private List<Renderer> targetList = new List<Renderer>();
    [SerializeField]private Color start;
    [SerializeField]private Color end;
    [SerializeField]private float speed = 1f;

    private float timer = 0f;
    private Color value = Color.white;
    private bool active = false;

    public void Active()
    {
        value = start;
        timer = 0f;
        active = true;

        this.enabled = true;
    }

    public void Update()
    {
        if(active)
        {
            timer += speed * Time.deltaTime;
            value = Color.Lerp(start,end,timer);
            if(timer >= 1f)
            {
                value = end;
                active = false;

                this.enabled = false;
            }

            SetEmission(value);
        }
    }

    public void SetEmission(Color value)
    {
        foreach(var target in targetList)
        {
            target.material.SetColor("_EmissionColor", value);
        }
    }
}
