using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VectorClasses
{
    [System.Serializable]
    public class Vector4C
    {
        [SerializeField]
        public float x = 0f, y = 0f, z = 0f, w = 0f;
        public static explicit operator Vector4(Vector4C v) {return new Vector4(v.x,v.y,v.z,v.w);}

        public Vector4C(){}
        public Vector4C(float x, float y,float z, float w){this.x = x; this.y = y; this.z = z; this.w = w;}
    }

    [System.Serializable]
    public class Vector3C
    {
        [SerializeField]
        public float x = 0f, y = 0f, z = 0f;
        public static explicit operator Vector3(Vector3C v) {return new Vector3(v.x,v.y,v.z);}

        public Vector3C(){}
        public Vector3C(float x, float y,float z){this.x = x; this.y = y; this.z = z;}
    }

    [System.Serializable]
    public class Vector3IntC
    {
        [SerializeField]
        public int x = 0, y = 0, z = 0;
        public static explicit operator Vector3Int(Vector3IntC v) {return new Vector3Int(v.x,v.y,v.z);}

        public Vector3IntC(){}
        public Vector3IntC(int x, int y,int z){this.x = x; this.y = y; this.z = z;}
    }

    [System.Serializable]
    public class Vector2C
    {
        [SerializeField]
        public float x = 0f, y = 0f;
        public static explicit operator Vector2(Vector2C v) {return new Vector2(v.x,v.y);}

        public Vector2C(){}
        public Vector2C(float x, float y,float z){this.x = x; this.y = y;}
    }

    [System.Serializable]
    public class Vector2IntC
    {
        [SerializeField]
        public int x = 0, y = 0, z = 0;
        public static explicit operator Vector2Int(Vector2IntC v) {return new Vector2Int(v.x,v.y);}

        public Vector2IntC(){}
        public Vector2IntC(int x, int y,int z){this.x = x; this.y = y;}
    }
}
