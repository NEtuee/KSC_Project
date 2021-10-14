using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontChecker : MonoBehaviour
{
    [SerializeField]private bool _overlap;
    [SerializeField] private LayerMask overlapMask;
    public bool Overlap => _overlap;

    private Collider[] _colliderBuffer = new Collider[10];

    private SphereCollider _collider; 

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
    }

    private void FixedUpdate()
    {
        int collisionCount = Physics.OverlapSphereNonAlloc(transform.position, _collider.radius, _colliderBuffer, overlapMask);
        _overlap = collisionCount != 0 ? true : false;
    }
}
