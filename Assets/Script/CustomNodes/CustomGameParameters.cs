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
        [SerializeField] PlayerUnit val;

        public override object value { get => val; set => val = (PlayerUnit)value; }
        public override Type GetValueType() => typeof(PlayerUnit);
    }

    [System.Serializable]
    public class FMODEmitterParameter : ExposedParameter
    {
        [SerializeField] FMODUnity.StudioEventEmitter val;

        public override object value { get => val; set => val = (FMODUnity.StudioEventEmitter)value; }
        public override Type GetValueType() => typeof(FMODUnity.StudioEventEmitter);
    }
}