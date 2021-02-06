using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignTestBoss : MonoBehaviour
{
    public Transform player;
    public Transform rayPoint;

    public List<CannonShot> cannons = new List<CannonShot>();

    public LayerMask rayMask;

    public float moveSpeed;
    public float turnSpeed;
    public float rushAngle = 30f;
    public float shotDistanceMin = 10f;
    public float shotDistanceMax = 100f;

    [SerializeField]private float _playerAngle;
    private float _spinTimer = 0.1f;
    private bool _rush = false;

    private Animator _animator;

    private Vector3 _moveFactor;
    private Vector3 _rushDirection;

    private RayEx headRay;

    private void Start()
    {
        headRay = new RayEx(new Ray(Vector3.zero,Vector3.zero),2f,rayMask);

        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        foreach(var cannon in cannons)
        {
            cannon.CanShot(player);
        }

        if(_rush)
        {
            _moveFactor = _rushDirection * moveSpeed * Time.deltaTime;

            RaycastHit hit;
            headRay.SetDirection(_rushDirection);
            headRay.Draw(rayPoint.position,Color.red);
            if(headRay.Cast(rayPoint.position,out hit))
            {
                _animator.SetBool("Crash",true);
                _animator.SetBool("Move",false);

                _animator.SetLayerWeight(1,0f);

                _spinTimer = 5f;
                _rush = false;
            }
        }
        else
        {
            if(_spinTimer != 0f)
            {
                _spinTimer -= Time.deltaTime;

                if(_spinTimer <= 0f)
                {
                    _spinTimer = 0f;
                    _animator.SetBool("Crash",false);
                    _animator.SetBool("Move",true);

                    _animator.SetLayerWeight(1,1f);
                }
                
                
            }
            else
            {
                var direction = player.position - transform.position;
                var forward = -Vector3.Cross(transform.forward,Vector3.up).normalized;
                direction.y = 0f;

                _playerAngle = Vector3.Angle(direction.normalized,forward);

                if(_playerAngle <= rushAngle)
                {
                    _rushDirection = forward;
                    _rushDirection.y = 0f;
                    _rush = true;
                }
                else
                {
                    var rotate = Quaternion.LookRotation(direction.normalized,Vector3.up);
                    rotate.eulerAngles -= new Vector3(0f,90f,0f);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation,rotate,Time.deltaTime * turnSpeed);
                }
            }

        }
    }

    private void FixedUpdate()
    {
        transform.position += _moveFactor * Time.deltaTime;
        _moveFactor = Vector3.zero;
    }

    public void Shot(int point)
    {
        var one = transform.position;
        var two = player.transform.position;
        one.y = 0f;
        two.y = 0f;

        var distance = Vector3.Distance(one,two);


        if(distance <= shotDistanceMax && distance >= shotDistanceMin)
        {
            cannons[point].Shot();
        }
    }
}
