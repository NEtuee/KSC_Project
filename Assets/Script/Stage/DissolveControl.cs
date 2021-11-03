using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveControl : MonoBehaviour
{
    public List<MeshRenderer> targets = new List<MeshRenderer>();
    private float _speed = 1f;

    private float _time = 0f;
    private float _start;
    private float _end;

    private bool _progress = false;

    public void Update()
    {
        if (!_progress)
            return;

        _time += Time.deltaTime;
        if(_time >= 1f)
        {
            _time = 1f;
            _progress = false;
        }

        var factor = Mathf.Lerp(_start, _end, _time);
        foreach (var item in targets)
        {
            item.material.SetFloat("Dissvole", factor);
        }
    }

    public void Active(float speed)
    {
        _speed = speed;

        _start = 1f;
        _end = 0f;
        _time = 0f;

        _progress = true;
    }

    public void Deactive(float speed)
    {
        _speed = speed;

        _start = 0f;
        _end = 1f;
        _time = 0f;

        _progress = true;
    }
}
