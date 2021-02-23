using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnTarget : MonoBehaviour
{
    public delegate void WhenTriggerOn();

    public bool oneshot = false;
    public WhenTriggerOn whenTriggerOn = () => { };

    private bool _triggered = false;

    private Vector2 screenPos;
    private bool inScreen = false;

    private Camera mainCamera;
    private void Awake()
    {
        mainCamera = Camera.main;
        gameObject.tag = "LockOnTarget";
    }

    public virtual void TriggerOn()
    {
        if (_triggered)
            return;

        if (oneshot)
        {
            _triggered = true;
        }

        whenTriggerOn();
    }

    private void FixedUpdate()
    {
        screenPos = mainCamera.WorldToScreenPoint(transform.position);
        if(screenPos.x >= 0.0f && screenPos.x <= GameManager.Instance.GetScreenWidth()
            &&screenPos.y >= 0.0f && screenPos.y <= GameManager.Instance.GetScreenHeight())
        {
            inScreen = true;
        }
        else
        {
            inScreen = false;
        }
    }

    public bool CanLockOn() { return inScreen; }
    public Vector2 GetScreenPosition() { return new Vector2(screenPos.x,screenPos.y); }


}
