using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genie_CenterShield : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color targetColor;
    public float speed;
    private Material _material;
    private Color _colorOrigin;

    private Color _start;
    private Color _end;

    private float _time;

    public void Start()
    {
        _material = GetComponent<Renderer>().material;
        _colorOrigin = _material.GetColor("_color");
    }

    public void Update()
    {
        if(_time < 1f)
        {
            var color = Color.Lerp(_start,_end,_time);
            _material.SetColor("_color",color);
            _time += Time.deltaTime;
        }
        
    }

    public void ToTarget()
    {
        _start = _colorOrigin;
        _end = targetColor;
        _time = 0f;
    }

    public void ToOrigin()
    {
        _start = targetColor;
        _end = _colorOrigin;
        _time = 0f;
    }
}
