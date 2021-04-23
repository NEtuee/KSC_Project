using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(transform.right * 20f * Time.deltaTime);
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(transform.right * -20f * Time.deltaTime);
        }
    }
}
