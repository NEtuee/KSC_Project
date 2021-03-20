using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

public class BossCtrl : MonoBehaviour
{
    public enum BossState
    {
        Wait,
        Turn,
        Rush,
        Groggy,
        GetUp
    }

    private NavMeshAgent agent;
    [SerializeField] private bool beActive;
    [SerializeField] private bool debuging;

    [SerializeField] private Transform target;
    [SerializeField] private CheckCollider forwardCheck;
    [SerializeField] private CheckCollider bellyCheck;
    [SerializeField] private List<EMPShield> footWeakPoint = new List<EMPShield>();
    private Vector3 targetPosition;
    private Rigidbody rigidbody;
    [SerializeField] private BossState state = BossState.Wait;
    private float elapsedTime = 0.0f;
    [SerializeField] private float waitTime = 3.0f;
    [SerializeField] private float turnTime = 3.0f;
    [SerializeField] private float turnSpeed = 4.0f;
    [SerializeField] private float rushSpeed = 10.0f;
    [SerializeField] private float groggyTime = 6.0f;
    private int wallLayer;


    private Vector3 targetDir;
    private Animator anim;
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        wallLayer = LayerMask.NameToLayer("Wall");

        GameManager.Instance.bossTransform = this.transform;

        if(forwardCheck != null)
        {
            forwardCheck.enterEvent += HitForward;
        }
        if(bellyCheck != null)
        {
            bellyCheck.enterEvent += HitByPumpingDrill;
        }
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (beActive == false)
            return;

        switch(state)
        {
            case BossState.Wait:
                {
                    elapsedTime += Time.fixedDeltaTime;
                    if(elapsedTime >= waitTime)
                    {
                        ChangeState(BossState.Turn);
                    }
                }
                break;
            case BossState.Turn:
                {
                    targetDir = target.position - transform.position;
                    targetDir.y = 0f;
                    Quaternion targetRot = Quaternion.LookRotation(targetDir, transform.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.fixedDeltaTime * turnSpeed);

                    elapsedTime += Time.fixedDeltaTime;
                    if(elapsedTime >= turnTime && Quaternion.Angle(transform.rotation,targetRot) < 5.0f)
                    {
                        ChangeState(BossState.Rush);
                    }
                }
                break;
            case BossState.Rush:
                {
                    //Vector3 targetVelocity = transform.forward * rushSpeed;
                    //Vector3 velocity = rigidbody.velocity;
                    //Vector3 velocityChange = targetVelocity - velocity;
                    //rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

                    rigidbody.MovePosition(transform.position+transform.forward * rushSpeed * Time.fixedDeltaTime);
                }
                break;
            case BossState.Groggy:
                {
                    elapsedTime += Time.fixedDeltaTime;
                    if(elapsedTime >= groggyTime)
                    {
                        state = BossState.Wait;
                        elapsedTime = 0.0f;
                        anim.SetTrigger("Return");
                    }
                }
                break;
        }    
    }

    private void ChangeState(BossState state)
    {
        elapsedTime = 0.0f;

        this.state = state;
        switch(state)
        {
            case BossState.Wait:
                {
                    anim.SetTrigger("Hit");
                }
                break;
            case BossState.Turn:
                {
                    if (Vector3.Dot(Vector3.Cross(transform.forward, target.position - transform.position), transform.up) < 0)
                    {
                        anim.SetTrigger("TurnLeft");
                    }
                    else
                    {
                        anim.SetTrigger("TurnRight");
                    }
                }
                break;
            case BossState.Rush:
                {
                    anim.SetTrigger("Rush");
                }
                break;
            case BossState.Groggy:
                {
                    anim.SetTrigger("Groggy");
                }
                break;
            case BossState.GetUp:
                {
                    anim.SetTrigger("Return");
                }
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        //switch (state)
        //{
        //    case BossState.Rush:
        //        {
        //            if(collision.gameObject.layer == wallLayer)
        //            {
        //                ChangeState(BossState.Groggy);
        //                return;
        //            }

        //            state = BossState.Wait;
        //            anim.SetTrigger("Hit");
        //        }
        //        break;
        //}
    }

    private void HitByPumpingDrill()
    {
        if(state != BossState.Rush)
        {
            return;
        }

        //Debug.Log("HitByPumping");
        //rigidbody.velocity = Vector3.zero;

        bool isGroggy = true;
        foreach (var weakPoint in footWeakPoint)
        {
            if (weakPoint.isOver == false)
            {
                isGroggy = false;
                break;
            }
        }

        if(isGroggy == true)
           ChangeState(BossState.Groggy);
        else
        {
            state = BossState.Wait;
            anim.SetTrigger("Stagger");
        }

    }

    private void HitForward()
    {
        if (state != BossState.Rush)
        {
            return;
        }

        //Debug.Log("HitByForward");
        //rigidbody.velocity = Vector3.zero;
        state = BossState.Wait;
        anim.SetTrigger("Hit");
    }

    private void Impulse()
    {
        impulseSource.GenerateImpulse();
    }

    private void OnGUI()
    {
        if(debuging == true)
        {
            GUI.Box(new Rect(10, 200, 300, 100), "Boss");
            //beActive = GUI.Toggle(new Rect(20, 40, 80, 20), beActive, "beActive");
            GUI.Label(new Rect(20, 230, 40, 20), "State :"); 
            GUI.Label(new Rect(60, 230, 40, 20), state.ToString());


        }
    }

    //private void OnCollisionStay(Collision collision)
    //{
    //    Debug.Log(collision.collider);


    //    switch (state)
    //    {
    //        case BossState.Rush:
    //            {
    //                if (collision.gameObject.layer == wallLayer)
    //                {
    //                    state = BossState.Groggy;
    //                    return;
    //                }

    //                state = BossState.Wait;
    //            }
    //            break;
    //    }
    //}
}
