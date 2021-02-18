using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD;
using FMODUnityResonance;

public class ETTSSTES : MonoBehaviour
{
    public StudioEventEmitter emit;

    void Start()
    {
        emit.EventInstance.getParameterByName("",out float value);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
