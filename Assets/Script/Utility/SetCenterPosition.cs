using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCenterPosition : MonoBehaviour
{
    public List<MeshRenderer> renderers = new List<MeshRenderer>();

    public void Progress()
    {
        foreach(var renderer in renderers)
        {
            var obj = new GameObject(renderer.name + "_Center");
            obj.transform.position = renderer.bounds.center;

            renderer.transform.SetParent(obj.transform);
        }

    }
}
