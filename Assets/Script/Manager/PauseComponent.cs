using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseComponent : MonoBehaviour
{
    protected bool isPause = false;

    void Start()
    {
        
    }

    public void SetPause(bool pause) { isPause = pause; }
}
