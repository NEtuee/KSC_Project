using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageObjectCreateTest : MonoBehaviour
{
    public GameObject target;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Instantiate(target);
        }
    }
}
