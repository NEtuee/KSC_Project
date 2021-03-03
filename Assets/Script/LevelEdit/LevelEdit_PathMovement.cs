using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_PathMovement : MonoBehaviour
{
    public string pathName;
    public float moveSpeed = 1f;
    public float turnAngle = 180f;
    public float turnAccuracy = 3f;

    public bool isLoop = false;
    public bool rotate = false;
    

    private LevelEdit_Controll _controll;
    private LevelEdit_PointManager.PathClass _path;

    private LevelEdit_MovePoint _startPoint;
    private LevelEdit_MovePoint _endPoint;

    private TimeCounterEx _timeCounter;

    private Vector3 _targetDirection;

    private float _targetLen = 0f;
    private int _currentPoint;

    private bool _lastPoint = false;


    public void Start()
    {
        _controll = FindObjectOfType(typeof(LevelEdit_Controll)) as LevelEdit_Controll;

        if(_controll == null)
        {
            Debug.Log("Controller not found");
            this.enabled = false;

            return;
        }

        _path = _controll.GetPath(pathName);

        if(_path == null)
        {
            Debug.Log("Controller not found");
            this.enabled = false;

            return;
        }

        _timeCounter = new TimeCounterEx();
        _timeCounter.InitTimer("timer",1f);


        _currentPoint = 0;
        _endPoint = _path.GetPoint(0);
        SetNextPoint();
    }

    public void Update()
    {
        LinearMovement();

        if(rotate)
        {
            var angle = Vector3.SignedAngle(transform.forward,_targetDirection,transform.up);

            if(MathEx.abs(angle) > turnAccuracy)
            {
                if(angle > 0)
                    Turn(true);
                else
                    Turn(false);
            }
        }
    }


    public void LinearMovement()
    {
        float factor = moveSpeed * Time.deltaTime / _targetLen;
        float time = _timeCounter.IncreaseTimerSelf("timer",1f,out bool limit, factor);

        if(limit)
        {
            if(_lastPoint && !isLoop)
            {
                transform.position = _endPoint.GetPoint();
                this.enabled = false;
                return;
            }

            _lastPoint = SetNextPoint();
            time = _timeCounter.InitTimer("timer");
        }
        
        transform.position = Vector3.Lerp(_startPoint.GetPoint(), _endPoint.GetPoint(),time);
    }

    public void Turn(bool isLeft)
    {
        transform.RotateAround(transform.position,transform.up,turnAngle * Time.deltaTime * (isLeft ? 1f : -1f));
    }

    public bool SetNextPoint()
    {
        _startPoint = _endPoint;
        _endPoint = _path.GetNextPoint(ref _currentPoint,out bool isEnd);

        _targetLen = Vector3.Distance(_startPoint.GetPoint(),_endPoint.GetPoint());
        _targetDirection = (_endPoint.GetPoint() - _startPoint.GetPoint()).normalized;

        return isEnd;
    }
}
