using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FIxedTest : MonoBehaviour
{
    [SerializeField] Transform fixedTr;
    [SerializeField] Transform fixedBone;

    [SerializeField] Transform fixedTr2;
    [SerializeField] Transform fixedBone2;

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

        fixedBone2.position = fixedTr2.position;
        fixedBone2.rotation = fixedTr2.rotation;
    }
}
