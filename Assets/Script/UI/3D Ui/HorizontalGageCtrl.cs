using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalGageCtrl : MonoBehaviour
{
    private Material _material;

    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
    }

    public void SetFactor(float factor)
    {
        _material.SetFloat("Factor", factor);
    }
}
