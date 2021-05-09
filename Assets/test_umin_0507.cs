using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_umin_0507 : MonoBehaviour
{

    public BezierLightning asdf;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            asdf.Active(new Vector3(0, 0, 0), new Vector3(2,2,0), 10, 1, 100);
        }
    }
}
