using System;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using VectorClasses;

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
    public class PathFollowGraphObjectParamter : ExposedParameter
    {
        [SerializeField] PathFollowGraphObjectBase val;

        public override object value { get => val; set => val = (PathFollowGraphObjectBase)value; }
        public override Type GetValueType() => typeof(PathFollowGraphObjectBase);
    }

    [System.Serializable]
    public class GraphObjectTransformParamter : ExposedParameter
    {
        [SerializeField] ObjectBase.ObjectTransform val;
        
        public override object value { get => val; set => val = value as ObjectBase.ObjectTransform; }
        public override Type GetValueType() => typeof(ObjectBase.ObjectTransform);
    }

    [System.Serializable]
    public class Vector2Parameter : ExposedParameter
    {
        [SerializeField] Vector2C val = new Vector2C();

        public override object value { get => val; set => val = (Vector2C)value; }
        public override Type GetValueType() => typeof(Vector2C);
    }

    [System.Serializable]
    public class Vector3Parameter : ExposedParameter
    {
        [SerializeField] public Vector3C val = new Vector3C();

        public override object value { get => val; set => val = (Vector3C)value; }
        public override Type GetValueType() => typeof(Vector3C);
    }

    [System.Serializable]
    public class Vector4Parameter : ExposedParameter
    {
        [SerializeField] Vector4C val = new Vector4C();

        public override object value { get => val; set => val = (Vector4C)value; }
        public override Type GetValueType() => typeof(Vector4C);
    }

    [System.Serializable]
    public class Vector2IntParameter : ExposedParameter
    {
        [SerializeField] Vector2IntC val = new Vector2IntC();

        public override object value { get => val; set => val = (Vector2IntC)value; }
        public override Type GetValueType() => typeof(Vector2IntC);
    }

    [System.Serializable]
    public class Vector3IntParameter : ExposedParameter
    {
        [SerializeField] Vector3IntC val = new Vector3IntC();

        public override object value { get => val; set => val = (Vector3IntC)value; }
        public override Type GetValueType() => typeof(Vector3IntC);
    }

    [System.Serializable]
    public class UnityEventParameter : ExposedParameter
    {
        [SerializeField] UnityEngine.Events.UnityEvent val = new UnityEngine.Events.UnityEvent();

        public override object value { get => val; set => val = (UnityEngine.Events.UnityEvent)value; }
        public override Type GetValueType() => typeof(UnityEngine.Events.UnityEvent);
    }

    [System.Serializable]
    public class MeshRendererParameter : ExposedParameter
    {
        [SerializeField] MeshRenderer val = new MeshRenderer();

        public override object value { get => val; set => val = (MeshRenderer)value; }
        public override Type GetValueType() => typeof(MeshRenderer);
    }
}