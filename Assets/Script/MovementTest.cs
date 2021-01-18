using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour 
{

    [SerializeField]private LayerMask groundLayer;
    [SerializeField]private float yVelocity = 1f;
    [SerializeField]private float limitAngle = 45f;

    private RayEx ray;

    void Start()
    {
        ray = new RayEx(new Ray(Vector3.zero,Vector3.down),5f,groundLayer);
    }
 
    void Update ()
    {
        Debug.DrawLine(transform.position,transform.position + Vector3.down * ray.Distance,Color.red);

        RaycastHit hit;
        if(ray.Cast(transform.position,out hit))
        {
            //미끄러지는 속도가 가속도에 영향 안받을거면 걍 0, -1, 0 벡터로 하면 됨
            var velocity = Vector3.down * yVelocity;
            //바닥 각도 체크
            var angle = Vector3.Angle(Vector3.down, hit.normal);

            //서 있을 수 있는 각도 넘어가면
            if(angle >= limitAngle)
            {
                //슬라이딩 벡터 구하기
                var slidingVector = MathEx.GetSlidingVector(velocity,hit.normal);


                Debug.DrawLine(hit.point,hit.point + slidingVector,Color.blue);
            }
        }
    }


 }