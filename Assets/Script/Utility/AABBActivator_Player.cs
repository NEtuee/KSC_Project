using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AABBActivator_Player : ObjectBase
{
    public Transform[] calculateTargets;

    public UnityEngine.Events.UnityEvent whenActive;
    public UnityEngine.Events.UnityEvent whenDeactive;

    public bool active = true;

    private Bounds _bounds;

    private Transform _player;
    private bool _active = true;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.set_setplayer, (x) =>
        {
            _player = ((PlayerUnit)x.data).transform;
        });
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);

        _bounds = new Bounds();
        UpdateBounds();

        whenDeactive?.Invoke();
    }

    public override void Progress(float deltaTime)
    {
        if (!active)
            return;

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
        var curr = _bounds.Contains(_player.position);
        if(_active != curr)
        {
            SetActive(_bounds.Contains(_player.position));
            _active = curr;

            if(curr)
            {
                whenActive?.Invoke();
            }
            else
            {
                whenDeactive?.Invoke();
            }
        }
        
    }

    public void SetActivator(bool value)
    {
        active = value;
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
