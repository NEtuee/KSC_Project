using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public delegate void Check();
    public Check ifPlayerIn = ()=>{};
    public Check ifPlayerOut = ()=>{};

    private bool _isPlayerChild = false;

    private void Update()
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            if(transform.GetChild(i).TryGetComponent<PlayerCtrl_State>(out var comp))
            {
                if(!_isPlayerChild)
                {
                    ifPlayerIn();
                }
                
                _isPlayerChild = true;
            }
            else
            {
                if(_isPlayerChild)
                {
                    ifPlayerOut();
                    _isPlayerChild = false;
                }
                
            }
        }
    }

    public void OnTriggerEnter(Collider coll)
    {
        if(coll.TryGetComponent<PlayerCtrl_Ver2>(out var comp))
            ifPlayerIn();
    }

    public void OnTriggerExit(Collider coll)
    {
        if(coll.TryGetComponent<PlayerCtrl_Ver2>(out var comp))
            ifPlayerOut();
    }
}
