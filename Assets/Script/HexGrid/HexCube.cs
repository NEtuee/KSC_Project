using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCube : MonoBehaviour
{
    public AnimationCurve outCurve;
    public AnimationCurve inCurve;

    public Vector3Int cubePoint;
    public Vector2Int axialPoint;
    public float cubeSize;
    public int key;
    public bool special = false;
    public float moveSpeed = 1f;

    private Vector3 _targetLocalPosition;
    private Vector3 _originalLocalPosition;
    private float _disapearTime;
    private float _moveTime;
    private bool _timer = false;
    private bool _isActive = true;

    
    private bool _inMove = false;
    private bool _outMove = false;
    private bool _inverseMove = false;
    private float _inverseMoveTime = 0f;
    private float _moveStartTime = 0f;
    private float _inoutMoveTime = 0f;
    private float _moveSpeed = 1f;

    private Collider _collider;
    private MeshRenderer _renderer;
    private System.Action _whenDisable;
    private System.Action _whenEnable;

    public void Start()
    {
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<MeshRenderer>();

        if(_renderer == null)
        {
            _renderer = GetComponentInChildren<MeshRenderer>();
        }

        _originalLocalPosition = transform.localPosition;
        _moveTime = 1f;
    }

    public void FixedUpdate()
    {
        Progress(Time.fixedDeltaTime);
    }

    public void Progress(float deltaTime)
    {
        if(_moveTime < 1f)
        {
            _moveTime += deltaTime * moveSpeed;
            if(_moveTime >= 1f)
                _moveTime = 1f;
            transform.localPosition = Vector3.Lerp(_originalLocalPosition,_originalLocalPosition + _targetLocalPosition,_moveTime);    
        }
        
        if(_inMove || _outMove)
        {
            if(_moveStartTime > 0f)
            {
                _moveStartTime -= deltaTime;
            }
            else if(_inoutMoveTime < 1f)
            {
                _inoutMoveTime += _moveSpeed * deltaTime;
                _inoutMoveTime = _inoutMoveTime >= 1f ? 1f : _inoutMoveTime;
                var pos = transform.localPosition;
                if(_inMove)
                    pos.y = inCurve.Evaluate(_inoutMoveTime);
                else
                    pos.y = outCurve.Evaluate(_inoutMoveTime);
                //pos.y = _inMove ? inCurve.Evaluate(_moveTime) : outCurve.Evaluate(_moveTime);
                transform.localPosition = pos;

                if(_inoutMoveTime >= 1f)
                {
                    _isActive = _inMove;
                    if(!_isActive)
                        _whenDisable?.Invoke();
                    else
                        _whenEnable?.Invoke();

                    if(_inverseMoveTime == 0f)
                    {
                        _inMove = false;
                        _outMove = false;
                    }
                    
                }
            }
            else if(_inverseMoveTime != 0f)
            {
                SetMove(!_inMove,_inverseMoveTime,_moveSpeed);
            }
        }

        if(_timer)
        {
            _disapearTime -= deltaTime;
            if(_disapearTime <= 0f)
            {
                _timer = false;
                _disapearTime = 0f;
                SetActive(true,false);
            }
        }
    }

    public MeshRenderer GetRenderer()
    {
        if(_renderer == null)
        {
            _renderer = GetComponent<MeshRenderer>();

            if(_renderer == null)
            {
                _renderer = GetComponentInChildren<MeshRenderer>();
            }
        }
        return _renderer;
    }

    public void SetTargetPosition(Vector3 local)
    {
        _targetLocalPosition = local;
        _moveTime = 0;
    }

    public bool IsActive()
    {
        return _isActive;
    }

    public void MoveToUp()
    {
        _inMove = false;
        _outMove = false;
        _isActive = true;
        _inverseMoveTime = 0f;
        var pos = transform.localPosition;
        pos.y = inCurve.Evaluate(1f);
        transform.localPosition = pos;
    }

    public void SetMove(bool active, float startTime, float speed, float inverseMoveTime = 0f, System.Action disable = null, System.Action enable = null)
    {
        _inMove = active;
        _outMove = !active;
        _isActive = false;
        _inverseMove = inverseMoveTime != 0f;
        _inverseMoveTime = inverseMoveTime;

        _moveStartTime = startTime;
        _inoutMoveTime = 0f;
        _moveSpeed = speed;

        _whenDisable = disable;
        _whenEnable = enable;
    }

    public void SetActive(bool active, bool timer, float disapearTime = 1f)
    {
        _collider.enabled = active;
        _renderer.enabled = active;
        _isActive = active;

        _timer = timer;
        _disapearTime = disapearTime;
    }

    public void Init(int q, int r, int mapSize, float size)
    {
        SetPoint(q,r);
        SetKeyFromAxial(mapSize);
        cubeSize = size;
    }
    
    public void SetPoint(int q, int r)
    {
        axialPoint = new Vector2Int(q,r);
        cubePoint = HexGridHelperEx.AxialToCube(axialPoint);
    }

    public void SetKeyFromAxial(int mapSize)
    {
        key = HexGridHelperEx.GetKeyFromAxial(axialPoint.x,axialPoint.y,mapSize);
        Debug.Log(key);
    }
}
