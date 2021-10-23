using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    public float verticalFactor;
    public float horizontalFactor;

    public float verticalTimeFactor;
    public float horizontalTimeFactor;

    private Vector3 _localPosition;

    private float _verticalTimer = 0f;
    private float _horizontalTimer = 0f;

    public void Start()
    {
        _localPosition = transform.localPosition;
    }

    public void FixedUpdate()
    {
        var pos = _localPosition;
        _verticalTimer += Time.fixedDeltaTime * verticalTimeFactor;
        _horizontalTimer += Time.fixedDeltaTime * horizontalTimeFactor;

        _verticalTimer = _verticalTimer >= 2f * Mathf.PI ? _verticalTimer - 2f * Mathf.PI : _verticalTimer;
        _horizontalTimer = _horizontalTimer >= 2f * Mathf.PI ? _horizontalTimer - 2f * Mathf.PI : _horizontalTimer;

        var vertical = MathEx.Lemniscate_Gerono(verticalFactor, _verticalTimer);;
        var horizontal = MathEx.Lemniscate_Gerono(horizontalFactor, _horizontalTimer);

        MathEx.Swap<float>(ref vertical.y, ref vertical.z);
        MathEx.Swap<float>(ref horizontal.x, ref horizontal.z);

        pos += vertical + horizontal;

        transform.localPosition = pos;
    }
}
