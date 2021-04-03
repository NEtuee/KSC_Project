using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScan : MonoBehaviour
{
    public Material scanMat;
    public float arc = 30f;
    public float scanSpeed = 80f;

    public bool scaning = false;
    public float maxRange = 1000.0f;
    public float range = 0f;

    void Update()
    {
        if (GameManager.Instance.PAUSE == true)
            return;

        if (scaning == true)
        {
            range += scanSpeed * Time.deltaTime;
            scanMat.SetFloat("_Distance", range);

            if (range > maxRange)
                scaning = false;
        }
    }

    public void ScanSetup(Vector3 start,Vector3 forward)
    {
        scaning = true;
        range = 0f;
        scanMat.SetFloat("_ScanArc", arc);
        scanMat.SetVector("_WorldSpaceScannerPos", start);
        scanMat.SetVector("_ForwardDirection", forward);
    }
}
