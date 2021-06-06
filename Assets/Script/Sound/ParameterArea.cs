using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterArea : MonoBehaviour
{
    public LayerMask targetLayer;
    public int parameterCode;
    public float enterValue;
    public float exitValue;

    public bool exitFade = false;

    private bool _fade = true;
    private float _timer = 0f;

    private List<Transform> _collsionList = new List<Transform>();

    public void Update()
    {
        if(_fade)
        {
            _timer += Time.deltaTime;
            GameManager.Instance.soundManager.SetGlobalParam(parameterCode,Mathf.Lerp(enterValue, exitValue,_timer));

            if(_timer >= 1f)
            {
                _fade = false;
            }
        }
    }

    public void OnTriggerEnter(Collider coll)
    {
        if(targetLayer == (targetLayer | (1<<coll.gameObject.layer)))
        {
            _fade = false;
            _collsionList.Add(coll.transform);
            GameManager.Instance.soundManager.SetGlobalParam(parameterCode,enterValue);
        }
       
    }

    public void OnTriggerExit(Collider coll)
    {
        if(targetLayer == (targetLayer | (1<<coll.gameObject.layer)))
        {
            _collsionList.Remove(coll.transform);
            if(_collsionList.Count == 0)
            {
                if(!exitFade)
                    GameManager.Instance.soundManager.SetGlobalParam(parameterCode,exitValue);
                else
                {
                    _fade = true;
                    _timer = 0f;
                }
            }
            
        }
        
    }
}
