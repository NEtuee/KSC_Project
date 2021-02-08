using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderPacker : MonoBehaviour
{
    public List<Collider> targetObjects = new List<Collider>();
    public bool deleteTarget = false;

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
                collider.size = scale;
            }

            targetObjects[i].transform.SetParent(package.transform);

            if(deleteTarget)
                DestroyImmediate(targetObjects[i]);
        }

        if(deleteTarget)
            targetObjects.Clear();
    } 
}
