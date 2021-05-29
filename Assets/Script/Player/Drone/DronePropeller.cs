using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePropeller : MonoBehaviour
{
    private Transform transform;
    [SerializeField] private float speed;

    private void Start()
    {
        transform = GetComponent<Transform>();
    }

    public void FixedUpdate()
    {
        transform.Rotate(Vector3.forward, speed * Time.fixedDeltaTime);
    }
}
