using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOrphan : MonoBehaviour
{
    public void Progress()
    {
        var pos = transform.position;
        var rot = transform.rotation;
        transform.parent = null;
        transform.position = pos;
        transform.rotation = rot;
    }
}
