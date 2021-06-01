using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaddyLongLegs_CutsceneMaster : MonoBehaviour
{
    public bool isMove = false;
    public bool culling = false;
    public Transform cullingTransform;

    public void SetCulling(bool value)
    {
        culling = value;
    }

    public void SetMove(bool value)
    {
        isMove = value;
    }
}
