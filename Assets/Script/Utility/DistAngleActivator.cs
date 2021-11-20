using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class DistAngleActivator : MonoBehaviour
{
    public Transform[] calculateTargets;

    public float limitAngle = 180f;
    public float limitDistance = 100f;

    public bool debug = false;

    private bool _forceDisable = false;
    private Transform _mainCameraTransform;

    public void Start()
    {
        _mainCameraTransform = Camera.main.transform;
    }

    public void Update()
    {
        
    }

    public void EnableCheck()
    {
        for(int i = 0; i < calculateTargets.Length; ++i)
        {
            calculateTargets[i].gameObject.SetActive(EnableCheck(calculateTargets[i]));
        }
    }

    public bool EnableCheck(Transform target)
    {
        var dir = (target.position - _mainCameraTransform.position).normalized;
        var angle = Vector3.Angle(_mainCameraTransform.forward, dir);

        if(angle <= limitAngle)
        {
            return Vector3.Distance(_mainCameraTransform.position, target.position) <= limitDistance;
        }
        else
        {
            return false;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!debug)
            return;

        if(Camera.main == null)
        {
            return;
        }
        else if(_mainCameraTransform == null)
        {
            _mainCameraTransform = Camera.main.transform;
        }
        var color = Handles.color;

        for (int i = 0; i < calculateTargets.Length; ++i)
        {
            if(EnableCheck(calculateTargets[i]))
            {
                Handles.color = Color.red;
            }
            else
            {
                Handles.color = Color.green;
            }

            Handles.DrawLine(_mainCameraTransform.position, calculateTargets[i].position);
        }

        Handles.color = color;

    }
#endif
}
