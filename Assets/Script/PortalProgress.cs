using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalProgress : MonoBehaviour
{
    public delegate void Delegate();

    public Transform portalPosition;
    public Transform groundPortalPoint;
    public Transform drone;
    public ScrapObject targetObject;

    public Transform droneObjectPoint;
    public Transform targetObjectPoint;

    public float speed = 0f;

    public float refillTimeFactorMin = 2f;
    public float refillTimeFactorMax = 3f;

    public Delegate whenHit = ()=>{};

    private Transform _targetPosition;

    private Vector3 _startPoint;
    private Vector3 _endPoint;

    private float _timer = 0f;
    private float _refillTime = 0f;

    private bool _refilling = false;
    private bool _progress = false;
    private bool _end = false;
    private bool _launch = false;

    private void Start()
    {
        targetObject.whenEat = DroneLaunch;
        //DroneLaunch();
        SetPortalToGround();
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position,_targetPosition.position,0.2f);
        _startPoint = transform.position;
        if(!_progress || !_launch)
        {
            return;
        }

        if(_refilling)
        {
            _timer += Time.deltaTime;
            if(_timer >= _refillTime)
            {
                _refilling = false;
                _timer = 0f;
            }

            return;
        }

        _timer += speed * Time.deltaTime;

        drone.position = Vector3.Lerp(_startPoint,_endPoint,_timer);

        if(_timer >= 1f)
        {
            _timer = 0f;

            var save = _startPoint;
            _startPoint = _endPoint;
            _endPoint = save;

            _progress = !_end;
            _end = _progress;

            drone.gameObject.SetActive(_progress);

            if(_end)
            {
                targetObject.transform.position = targetObjectPoint.position;
                targetObject.transform.SetParent(targetObjectPoint);

                targetObject.isReady = true;
            }
        }
    }

    public void WhenHit()
    {
        if(_launch)
            whenHit();
    }

    public void SetPortalToGround()
    {
        _targetPosition = groundPortalPoint;
    }

    public void DroneLaunch()
    {
        _refillTime = Random.Range(refillTimeFactorMin,refillTimeFactorMax);

        _startPoint = transform.position;
        _endPoint = targetObjectPoint.position;

        drone.gameObject.SetActive(true);
        drone.position = transform.position;

        targetObject.gameObject.SetActive(true);
        targetObject.transform.position = droneObjectPoint.position;
        targetObject.transform.SetParent(droneObjectPoint);

        _timer = 0f;

        _refilling = true;
        _progress = true;
        _end = false;
    }

    public void OnTriggerStay(Collider coll)
    {
        if(coll.tag == "Player")
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                var con = coll.GetComponent<TestPortalBatteryScript>();

                if(con.battery > 0)
                {
                    con.battery--;
                    _launch = _progress = true;

                    _targetPosition = portalPosition;

                    DroneLaunch();
                }
            }
        }
    }

}
