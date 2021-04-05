using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderPacker : MonoBehaviour
{
    public List<Collider> targetObjects = new List<Collider>();
    public bool deleteTarget = false;
    public Vector3 colliderScale = Vector3.one;

    public void Normalize()
    {
        foreach(var obj in targetObjects)
        {
            var scale = obj.transform.localScale;

            if(obj.GetType() == typeof(BoxCollider))
            {
                var boxs = obj.GetComponents<BoxCollider>();

                foreach(var box in boxs)
                {
                    var center = box.center;
                    var size = box.size;
    
                    center.x *= scale.x * colliderScale.x;
                    center.y *= scale.y * colliderScale.y;
                    center.z *= scale.z * colliderScale.z;
    
                    size.x *= scale.x * colliderScale.x;
                    size.y *= scale.y * colliderScale.y;
                    size.z *= scale.z * colliderScale.z;
    
                    box.center = center;
                    box.size = size;
                }

            }

            obj.transform.localScale = Vector3.one;
        }

        if(deleteTarget)
            targetObjects.Clear();
    }

    public void Pack()
    {
        for(int i = 0; i < targetObjects.Count; ++i)
        {
            var position = targetObjects[i].transform.localPosition;
            var scale = targetObjects[i].transform.localScale;
            var rotation = targetObjects[i].transform.localRotation;

            var package = new GameObject(targetObjects[i].name + "_Collider");
            package.tag = targetObjects[i].tag;
            package.layer = targetObjects[i].gameObject.layer;
            package.transform.SetParent(targetObjects[i].transform.parent);
            package.transform.localPosition = position;
            package.transform.localRotation = rotation;

            for(int j = 0; j < targetObjects[i].transform.childCount; ++i)
            {
                targetObjects[i].transform.GetChild(j).SetParent(package.transform);
            }

            if(targetObjects[i].GetType() == typeof(BoxCollider))
            {
                var collider = package.AddComponent<BoxCollider>();
                collider.size = MathEx.MultiplyElems(scale, colliderScale);
            }

            targetObjects[i].transform.SetParent(package.transform);

            if(deleteTarget)
                DestroyImmediate(targetObjects[i]);
        }

        if(deleteTarget)
            targetObjects.Clear();
    } 
}
