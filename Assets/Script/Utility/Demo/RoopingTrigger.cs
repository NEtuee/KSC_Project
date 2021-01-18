using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoopingTrigger : MonoBehaviour
{
    [SerializeField] private Transform startPos;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = startPos.position;
    }
}
