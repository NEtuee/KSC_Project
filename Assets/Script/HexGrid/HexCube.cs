using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCube : MonoBehaviour
{
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

    private Collider _collider;
    private MeshRenderer _renderer;

    public void Start()
    {
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<MeshRenderer>();

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
