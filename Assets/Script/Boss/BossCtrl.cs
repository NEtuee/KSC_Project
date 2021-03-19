using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rigidbody = GetComponent<Rigidbody>();
        agent.enabled = false;
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        wallLayer = LayerMask.NameToLayer("Wall");
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
                        elapsedTime = 0.0f;
                        state = BossState.Turn;
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
                        elapsedTime = 0.0f;
                        state = BossState.Rush;
                    }
                }
                break;
            case BossState.Rush:
                {
                    Vector3 targetVelocity = transform.forward * rushSpeed;
                    Vector3 velocity = rigidbody.velocity;
                    Vector3 velocityChange = targetVelocity - velocity;
                    rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
                }
                break;
            case BossState.Groggy:
                {
                    elapsedTime += Time.fixedDeltaTime;
                    if(elapsedTime >= groggyTime)
                    {
                        state = BossState.Wait;
                        elapsedTime = 0.0f;
                    }
                }
                break;
        }    
    }

    private void OnCollisionEnter(Collision collision)
    {

        switch (state)
        {
            case BossState.Rush:
                {
                    if(collision.gameObject.layer == wallLayer)
                    {
                        state = BossState.Groggy;
                        elapsedTime = 0.0f;
                        return;
                    }

                    state = BossState.Wait;
                }
                break;
        }
    }

    private void OnGUI()
    {
        if(debuging == true)
        {
            GUI.Box(new Rect(10, 200, 300, 100), "Boss");
            //beActive = GUI.Toggle(new Rect(20, 40, 80, 20), beActive, "beActive");
            GUI.Label(new Rect(20, 230, 40, 20), "State :"); 
            GUI.Label(new Rect(40, 230, 80, 20), state.ToString());


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
