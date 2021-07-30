using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceChecker : MonoBehaviour
{
    [SerializeField] private bool overlap;
    private CapsuleCollider _capsuleCollider;
    private Vector3 _pos1;
    [SerializeField] private Vector3 pos1Offset;
    [SerializeField] private Vector3 pos2Offset;
    private Vector3 _pos2;
    private Collider[] _colliderBuffer = new Collider[10];
    public List<string> detectGameObjectNameList = new List<string>();

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
        _pos1 = transform.position + transform.TransformDirection(pos1Offset);
        _pos2 = transform.position + transform.TransformDirection(pos2Offset);

        int collisionCount = Physics.OverlapCapsuleNonAlloc(_pos1, _pos2, _capsuleCollider.radius,_colliderBuffer);
#if UNITY_EDITOR

        if (collisionCount != 0)
        {
            for(int i = 0; i<_colliderBuffer.Length; i++)
            {
                if (_colliderBuffer[i] == null)
                    break;

                detectGameObjectNameList.Insert(0, _colliderBuffer[i].name);
            }

            if (detectGameObjectNameList.Count > 10)
            {
                detectGameObjectNameList.RemoveRange(10, detectGameObjectNameList.Count - 10);
            }
        }
        else
        {
            detectGameObjectNameList.Clear();
        }
#endif

        overlap = collisionCount != 0;
    }

    public void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_pos1, _capsuleCollider.radius);
        Gizmos.DrawWireSphere(_pos2, _capsuleCollider.radius);
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
