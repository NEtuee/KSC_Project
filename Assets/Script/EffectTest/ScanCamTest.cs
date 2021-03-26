using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanCamTest : MonoBehaviour
{
    public Transform forward;
    public Transform scanStart;
    public Material scanMat;
    public float arc = 30f;
    public float scanSpeed = 80f;

    private float _range = 0f;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            _range = 0f;
            scanMat.SetFloat("_ScanArc",arc);
            scanMat.SetVector("_WorldSpaceScannerPos", scanStart.position);
            scanMat.SetVector("_ForwardDirection", forward.forward);
        }

        _range += scanSpeed * Time.deltaTime;
        scanMat.SetFloat("_Distance", _range);
    }
}
