using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1_WallDrone : MonoBehaviour
{
    public float speed;
    public float moveFactor;

    public bool sinGraph = false;
    
    private Vector3 _localOrigin;
    private float _time = 0f;

    public void Start()
    {
        _localOrigin = transform.localPosition;
        _time = 0f;
    }

    public void FixedUpdate()
    {
        _time += speed * Time.fixedDeltaTime;
        _time = _time >= Mathf.PI * 2f ? _time - Mathf.PI * 2f : _time;
        var tri = sinGraph ? Mathf.Sin(_time) : Mathf.Sin(_time + Mathf.PI);

        var pos = _localOrigin;
        pos.z = tri * moveFactor;

        transform.localPosition = pos;
    }
}
