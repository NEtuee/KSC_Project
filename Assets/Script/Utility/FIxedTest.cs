using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIxedTest : MonoBehaviour
{
    [SerializeField] Transform fixedTr;
    [SerializeField] Transform fixedBone;

    Vector3 originalPos;
    Quaternion originalRot;

    private void Awake()
    {
        originalPos = fixedBone.position;
        originalRot = fixedBone.rotation;
    }

    private void Update()
    {
        fixedBone.position = fixedTr.position;
        fixedBone.rotation = fixedTr.rotation;
    }
}
