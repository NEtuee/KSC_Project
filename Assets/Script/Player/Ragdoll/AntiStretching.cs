using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiStretching : MonoBehaviour
{
    private Vector3 startPosition;
    [SerializeField] private bool isHand = false;
    void Start()
    {
        startPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        if (isHand == false)
        {
            transform.localPosition = startPosition;
        }
    }
}
