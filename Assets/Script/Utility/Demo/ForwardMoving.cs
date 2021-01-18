using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardMoving : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
}
