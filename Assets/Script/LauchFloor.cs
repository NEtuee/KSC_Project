using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauchFloor : MonoBehaviour
{
    [SerializeField] private FloorControl fc;
    void Start()
    {
        fc.Launch();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
