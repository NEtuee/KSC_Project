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
        GetUp,
        LookPatrol,
        GoToPatrol,
        Patrol,
        Dead
    }

    private NavMeshAgent agent;
    [SerializeField] private bool beActive;
    [SerializeField] private bool debuging;
    [SerializeField] private Transform target;
    [SerializeField] private CheckCollider forwardCheck;
    [SerializeField] private CheckCollider bellyCheck;
    [SerializeField] private List<EMPShield> footWeakPoint = new List<EMPShield>();
    [SerializeField] private List<EMPShield> weakPoints = new List<EMPShield>();
    [SerializeField] private GameObject coreHideObejct;
    [SerializeField] private GameObject coreHideCollider;
    [SerializeField] private EMPShield coreObject;
    [SerializeField] private LevelEdit_PointManager pointManager;
    private LevelEdit_PointManager.PathClass path;
    [SerializeField] private CheckBack checkBack;

    private int currentPointIndex;
    private Vector3 targetPosition;
    private Rigidbody rigidbody;
    public BossState state = BossState.Wait;
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

        path = pointManager.FindPath("BossDummy_1");

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
        if (beActive == false)
            return;

        if(Input.GetKeyDown(KeyCode.H))
        {
            foreach (var weakPoint in weakPoints)
            {
                weakPoint.Hit();
            }
        }

        switch (state)
        {
            case BossState.Wait:
                {
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime >= waitTime)
                    {
                        ChangeState(BossState.Turn);
                    }
                }
                break;
            case BossState.Turn:
                {
                    if (checkBack.playerCheck == true)
                    {
                        Quaternion lookPatrolRot = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
                        if (transform.rotation != lookPatrolRot)
                        {
                            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookPatrolRot, Time.deltaTime * turnSpeed);
                        }
                        else
                        {
                            state = BossState.Patrol;
                        }
                    }
                    else
                    {
                        targetDir = target.position - transform.position;
                        targetDir.y = 0f;
                        Quaternion targetRot = Quaternion.LookRotation(targetDir, transform.up);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.deltaTime * turnSpeed);

                        elapsedTime += Time.deltaTime;
                        if (elapsedTime >= turnTime && Quaternion.Angle(transform.rotation, targetRot) < 5.0f)
                        {
                            ChangeState(BossState.Rush);
                        }
                    }
                }
                break;
            case BossState.Rush:
                {
                    //Vector3 targetVelocity = transform.forward * rushSpeed;
                    //Vector3 velocity = rigidbody.velocity;
                    //Vector3 velocityChange = targetVelocity - velocity;
                    //rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

                    //rigidbody.MovePosition(transform.position+transform.forward * rushSpeed * Time.fixedDeltaTime);
                    //transform.Translate(transform.forward * rushSpeed * Time.fixedDeltaTime);
                    transform.position += transform.forward * rushSpeed * Time.deltaTime;
                }
                break;
            case BossState.Groggy:
                {
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime >= groggyTime)
                    {
                            state = BossState.GetUp;
                            elapsedTime = 0.0f;
                            anim.SetTrigger("Return");               
                    }
                }
                break;
            case BossState.LookPatrol:
                {
                    Quaternion lookPatrolRot = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
                    if (transform.rotation != lookPatrolRot)
                    {
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookPatrolRot, Time.deltaTime * turnSpeed);
                    }
                    else
                    {
                        state = BossState.GoToPatrol;
                    }
                }
                break;
            case BossState.GoToPatrol:
                {
                    if (transform.position != targetPosition)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, rushSpeed * Time.deltaTime);
                    }
                    else
                    {
                        state = BossState.Turn;
                        anim.SetTrigger("Turn");
                        bool isEnd;
                        targetPosition = path.GetNextPoint(ref currentPointIndex, out isEnd).GetPoint();
                        if(isEnd == true)
                        {
                            currentPointIndex = 0;
                        }
                    }
                }
                break;
            case BossState.Patrol:
                {
                    if(checkBack.playerCheck == false)
                    {
                        ChangeState(BossState.Wait);
                        return;
                    }

                    if (transform.position != targetPosition)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, targetPosition, rushSpeed * Time.deltaTime);
                    }
                    else
                    {
                        state = BossState.LookPatrol;
                        anim.SetTrigger("Turn");
                        bool isEnd;
                        targetPosition = path.GetNextPoint(ref currentPointIndex, out isEnd).GetPoint();
                        if (isEnd == true)
                        {
                            currentPointIndex = 0;
                        }
                    }
                }
                break;
        }
    }

    private void EndReturn()
    {
        if (checkBack.playerCheck == true)
        {
            state = BossState.LookPatrol;
            anim.SetTrigger("Turn");
            targetPosition = path.FindNearestPoint(transform.position, out currentPointIndex).GetPoint();
        }
        else
        {
            state = BossState.Wait;
        }
    }

    private void FixedUpdate()
    {
        if (state == BossState.Dead)
            return;

        if (coreHideCollider.activeSelf == true)
        {
            foreach (var weakPoint in weakPoints)
            {
                if (weakPoint.isOver == false)
                    return;
            }

            coreHideObejct.SetActive(false);
            coreHideCollider.SetActive(false);
            coreObject.gameObject.SetActive(true);
        }
        else
        {
            if (coreObject.isOver == true)
            {
                state = BossState.Dead;
                anim.SetTrigger("Dead");
            }
        }
        //if (beActive == false)
        //    return;

        //switch(state)
        //{
        //    case BossState.Wait:
        //        {
        //            elapsedTime += Time.fixedDeltaTime;
        //            if(elapsedTime >= waitTime)
        //            {
        //                ChangeState(BossState.Turn);
        //            }
        //        }
        //        break;
        //    case BossState.Turn:
        //        {
        //            targetDir = target.position - transform.position;
        //            targetDir.y = 0f;
        //            Quaternion targetRot = Quaternion.LookRotation(targetDir, transform.up);
        //            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.fixedDeltaTime * turnSpeed);

        //            elapsedTime += Time.fixedDeltaTime;
        //            if(elapsedTime >= turnTime && Quaternion.Angle(transform.rotation,targetRot) < 5.0f)
        //            {
        //                ChangeState(BossState.Rush);
        //            }
        //        }
        //        break;
        //    case BossState.Rush:
        //        {
        //            //Vector3 targetVelocity = transform.forward * rushSpeed;
        //            //Vector3 velocity = rigidbody.velocity;
        //            //Vector3 velocityChange = targetVelocity - velocity;
        //            //rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        //            //rigidbody.MovePosition(transform.position+transform.forward * rushSpeed * Time.fixedDeltaTime);
        //            //transform.Translate(transform.forward * rushSpeed * Time.fixedDeltaTime);
        //            transform.position += transform.forward * rushSpeed * Time.fixedDeltaTime;
        //        }
        //        break;
        //    case BossState.Groggy:
        //        {
        //            elapsedTime += Time.fixedDeltaTime;
        //            if (elapsedTime >= groggyTime)
        //            {
        //                state = BossState.Wait;
        //                elapsedTime = 0.0f;
        //                anim.SetTrigger("Return");
        //            }
        //        }
        //        break;
        //}    
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
