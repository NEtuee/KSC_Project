using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpecialSpearCtrl : MonoBehaviour
{
    [SerializeField]private Transform target;
    [SerializeField]private TrailRenderer trail;
    
    [SerializeField]private float speed = 1f;
    [SerializeField]private UnityEvent whenArriveTarget = new UnityEvent();
    private float linearDistance;
    private float targetAngle = 0f;
    private float curveLength = 0f;
    private Vector3 targetDirection;

    private Vector3 startPoint;
    private Vector3 bezierPoint0;
    private Vector3 bezierPoint1;
    
    private float timer = 0f;

    bool launch = false;

    public void Start()
    {
        Launch(target);
        AddListener(DestroySelf);
    }

    public void Update()
    {
        if(launch)
        {
            Progress();
        }

    }

    public void Launch(Transform t)
    {
        target = t;
        launch = true;

        startPoint = transform.position;
        linearDistance = Vector3.Distance(startPoint,target.position);
        targetDirection = (target.position - startPoint).normalized;
        targetAngle = Vector3.Dot(transform.forward,targetDirection);

        var correction = (180f / (180f - targetAngle)) * (linearDistance * 10f);
        bezierPoint0 = startPoint + (transform.forward * (linearDistance * 0.5f));
        bezierPoint1 = target.position + (target.forward * (linearDistance * 0.5f));

        curveLength = MathEx.GetBezierLengthStupidWay(startPoint,bezierPoint0,bezierPoint1,target.position,20);
    }

    public void SetTimerZero()
    {
        timer = 0f;
    }

    public void DestroySelf()
    {
        trail.gameObject.transform.parent = null;
        Destroy(trail.gameObject,10f);
        Destroy(gameObject);
    }

    public void AddListener(UnityAction action)
    {
        whenArriveTarget.AddListener(action);
    }

    public void Progress()
    {
        timer += (speed * Time.deltaTime) / curveLength;
        var nextPoint = MathEx.GetPointOnBezierCurve(startPoint,bezierPoint0,bezierPoint1,target.position,timer);
        var forward = (nextPoint - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(forward,Vector3.up);
        transform.position = nextPoint;

        if(timer >= 1f)
        {
            whenArriveTarget.Invoke();
        }
    }
}
