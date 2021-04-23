using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreateor_test : MonoBehaviour
{
    public List<GameObject> objectList = new List<GameObject>();
    public KeyCode spawnKeyCode;

    public float deleteTime;
    
    public void Update()
    {
        if (Input.GetKeyDown(spawnKeyCode))
        {
            foreach (var obj in objectList)
            {
                var spawn = Instantiate(obj, Vector3.zero, Quaternion.identity);
                spawn.transform.SetParent(this.transform);
                spawn.transform.localPosition = Vector3.zero;
                
                Destroy(spawn,deleteTime);
            }
        }
    }
}
