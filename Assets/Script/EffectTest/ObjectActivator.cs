using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActivator : MonoBehaviour
{
    [SerializeField]private List<GameObject> objList = new List<GameObject>();

    public void Active()
    {
        foreach(var obj in objList)
        {
            obj.SetActive(true);
        }
    }

    public void Deactive()
    {
        foreach(var obj in objList)
        {
            obj.SetActive(false);
        }
    }
}
