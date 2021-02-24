using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreTarget : MonoBehaviour
{
    public LockOnTarget target;
    public Material lit;
    public bool check = false;
    public int count = 3;
    
    private void Start()
    {
        target.whenTriggerOn += Attacked;
    }

    public void Attacked()
    {
        if(--count == 0)
        {
            check = true;
            GetComponent<MeshRenderer>().material = lit;
        }
    }
}
