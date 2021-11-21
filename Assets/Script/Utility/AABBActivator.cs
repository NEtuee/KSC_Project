using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AABBActivator : MonoBehaviour
{
    public Transform[] calculateTargets;

    private Bounds _bounds;
    private Transform _mainCameraTransform;

    private void Start()
    {
        _bounds = new Bounds();
        _mainCameraTransform = Camera.main.transform;
        UpdateBounds();
    }

    private void Update()
    {
        UpdateBounds();
        EnableCheck();
    }

    public void UpdateBounds()
    {
        _bounds.center = transform.position;
        _bounds.extents = transform.localScale;
    }

    public void EnableCheck()
    {
        SetActive(_bounds.Contains(_mainCameraTransform.position));
    }

    public void SetActive(bool value)
    {
        for(int i = 0; i < calculateTargets.Length; ++i)
        {
            calculateTargets[i].gameObject.SetActive(value);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {

        var color = Handles.color;

        if(_bounds == null)
        {
            _bounds = new Bounds();
        }

        Handles.color = Color.red;

        UpdateBounds();

        var min = _bounds.min;
        var max = _bounds.max;

        Handles.DrawLine(min, new Vector3(max.x,min.y,min.z));
        Handles.DrawLine(min, new Vector3(min.x,max.y,min.z));
        Handles.DrawLine(min, new Vector3(min.x,min.y,max.z));

        Handles.DrawLine(max, new Vector3(min.x,max.y,max.z));
        Handles.DrawLine(max, new Vector3(max.x,min.y,max.z));
        Handles.DrawLine(max, new Vector3(max.x,max.y,min.z));

        Handles.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(min.x, max.y, max.z));
        Handles.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(max.x, max.y, min.z));

        Handles.DrawLine(new Vector3(max.x, min.y, max.z), new Vector3(min.x, min.y, max.z));
        Handles.DrawLine(new Vector3(max.x, min.y, max.z), new Vector3(max.x, min.y, min.z));

        Handles.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(min.x, max.y, max.z));
        Handles.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, max.y, min.z));

        Handles.color = color;

    }
#endif
}
