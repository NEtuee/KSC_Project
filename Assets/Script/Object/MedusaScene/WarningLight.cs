using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLight : MonoBehaviour
{
    private Material _matrial;

    private void Start()
    {
        _matrial = GetComponent<Renderer>().material;
        _matrial.DisableKeyword("_EMISSION");
    }

    public void OnLight()
    {
        _matrial.EnableKeyword("_EMISSION");
    }
}
