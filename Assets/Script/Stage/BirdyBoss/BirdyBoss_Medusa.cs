using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdyBoss_Medusa : MonoBehaviour
{
    public StateProcessor stateProcessor;

    private bool _spawn = false;
    public void Start()
    {
        stateProcessor.whenStateChanged += (x)=>{
            if(x.stateIdentifier == "CenterMove")
            {
                if(!_spawn)
                {
                    gameObject.SetActive(false);
                }
            }
        };
    }
    public void Launch()
    {
        _spawn = true;
        stateProcessor.StateChange("CenterMove");
        _spawn = false;
    }
}
