using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingMove : MonoBehaviour
{
    [SerializeField] private float floatingSpeed;
    private Vector3 startPosition;
    [SerializeField]private float range;

    private void Start()
    {
        startPosition = transform.localPosition;
    }
    void Update()
    {
        float adjust = Mathf.Sin(Time.time * floatingSpeed);
        transform.localPosition = startPosition + Vector3.up *range* adjust;
    }
}
