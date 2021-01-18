using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialFloatModifier : MonoBehaviour
{
    [SerializeField]private List<Renderer> targetList = new List<Renderer>();
    [SerializeField]private string floatName;
    [SerializeField]private float start;
    [SerializeField]private float end;
    [SerializeField]private float speed = 1f;

    private float timer = 0f;
    private float value = 0f;
    private float dist = 0f;
    private bool active = false;

    public void Active()
    {
        value = start;
        dist = MathEx.distance(start,end);
        Debug.Log(dist);
        timer = 0f;
        active = true;

        this.enabled = true;
    }

    public void Update()
    {
        if(active)
        {
            timer += speed * Time.deltaTime / dist;
            value = Mathf.Lerp(start,end,timer);
            if(timer >= 1f)
            {
                value = end;
                active = false;

                this.enabled = false;
            }

            SetFloat(value);
        }
    }

    public void SetFloat(float value)
    {
        foreach(var target in targetList)
        {
            target.material.SetFloat(floatName,value);
        }
    }
}
