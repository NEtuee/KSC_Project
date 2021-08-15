using System;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using VectorClasses;

namespace GraphProcessor
{
    [System.Serializable]
    public class PlayerParamter : ExposedParameter
    {
        [SerializeField] PlayerCtrl_Ver2 val;

        public override object value { get => val; set => val = (PlayerCtrl_Ver2)value; }
        public override Type GetValueType() => typeof(PlayerCtrl_Ver2);
    }

    [System.Serializable]
    public class FMODEmitterParameter : ExposedParameter
    {
        [SerializeField] FMODUnity.StudioEventEmitter val;

        public override object value { get => val; set => val = (FMODUnity.StudioEventEmitter)value; }
        public override Type GetValueType() => typeof(FMODUnity.StudioEventEmitter);
    }
}