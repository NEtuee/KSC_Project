using System;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;

namespace GraphProcessor
{
    [System.Serializable]
    public class GraphObjectParamter : ExposedParameter
    {
        [SerializeField] GraphObjectBase val;

        public override object value { get => val; set => val = (GraphObjectBase)value; }
        public override Type GetValueType() => typeof(GraphObjectBase);
    }

    [System.Serializable]
    public class GraphObjectTransformParamter : ExposedParameter
    {
        [SerializeField] ObjectBase.ObjectTransform val;
        
        public override object value { get => val; set => val = (ObjectBase.ObjectTransform)value; }
        public override Type GetValueType() => typeof(GraphObjectBase);
    }
}