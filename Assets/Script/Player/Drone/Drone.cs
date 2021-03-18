using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public enum DroneState { Default, Approach, Collect, Return}

    [SerializeField] private Transform target;
    [SerializeField] private DroneState state;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rightOffset = 0.0f;
    [SerializeField] private float upOffset = 0.0f;
    [SerializeField] private float forwardOffset = 0.0f;
    [SerializeField] private float collectRequiredTime = 1f;
    private float collectStartTime;
    
    private Vector3 approachTargetPosition;
    private Stack<Vector3> orderList = new Stack<Vector3>();
    private Transform mainCam;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (((PlayerCtrl_Ver2)GameManager.Instance.player).updateMethod != UpdateMethod.FixedUpdate)
            return;

        UpdateDrone(Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        if (((PlayerCtrl_Ver2)GameManager.Instance.player).updateMethod != UpdateMethod.Update)
            return;

        UpdateDrone(Time.deltaTime);
    }

    private void UpdateDrone(float deltaTime)
    {
        switch(state)
        {
            case DroneState.Default:
                {
                    Vector3 targetPosition = (target.forward * forwardOffset + target.right * rightOffset + target.up * upOffset) + target.position;
                    transform.position = Vector3.Lerp(transform.position, targetPosition, deltaTime * 5f);
                }
                break;
            case DroneState.Approach:
                {
                    transform.position = Vector3.MoveTowards(transform.position, approachTargetPosition, moveSpeed * deltaTime);
                    if((transform.position - approachTargetPosition).sqrMagnitude < 1f)
                    {
                        state = DroneState.Collect;
                        collectStartTime = Time.time;
                    }
                }
                break;
            case DroneState.Collect:
                {
                    if(Time.time - collectStartTime > collectRequiredTime)
                    {
                        if(orderList.Count == 0)
                        {
                            state = DroneState.Return;
                        }
                        else
                        {
                            state = DroneState.Approach;
                            approachTargetPosition = orderList.Pop();
                        }
                    }
                }
                break;
            case DroneState.Return:
                {
                    Vector3 targetPosition = (target.forward * forwardOffset + target.right * rightOffset + target.up * upOffset) + target.position;
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, deltaTime * moveSpeed);

                    if((transform.position - targetPosition).sqrMagnitude <=2.0f)
                    {
                        state = DroneState.Default;
                    }
                }
                break;
        }
    }

    public void OrderApproch(Vector3 targetPosition)
    {
        if (state == DroneState.Default || state == DroneState.Return)
        {
            state = DroneState.Approach;
            approachTargetPosition = targetPosition;
        }
        else
        {
            orderList.Push(targetPosition);
        }
    }
}
