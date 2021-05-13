using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceChecker : MonoBehaviour
{
    [SerializeField] private bool overlap;
    private CapsuleCollider _capsuleCollider;
    private Vector3 _pos1;
    private Vector3 _pos2;
    private Collider[] _colliderBuffer = new Collider[10];

    private void Start()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();

        if (_capsuleCollider == null)
        {
            Debug.LogWarning("Not Exits Capsule Collider");
        }
    }

    private void FixedUpdate()
    {
        float height = _capsuleCollider.height;
        _pos1 = transform.position;
        _pos2 = _pos1 + transform.up * height;

        int collisionCount = Physics.OverlapCapsuleNonAlloc(_pos1, _pos2, _capsuleCollider.radius,_colliderBuffer);
        overlap = collisionCount != 0;
    }

    // private void OnTriggerStay(Collider other)
    // {
    //     overlap = true;
    // }
    //
    // private void OnTriggerExit(Collider other)
    // {
    //     overlap = false;
    // }

    public bool Overlapped() { return overlap; }
}
