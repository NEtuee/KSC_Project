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
    
    private Transform approachTarget;
    private Stack<Transform> orderList = new Stack<Transform>();
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
                    targetPosition = Vector3.Lerp(transform.position, targetPosition, deltaTime * 5f);
                    Vector3 lookDir = targetPosition - transform.position;
                    Quaternion targetRot;
                    if (lookDir != Vector3.zero)
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), 10.0f * deltaTime);
                    else
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 10.0f * deltaTime);

                    transform.SetPositionAndRotation(targetPosition, targetRot);
                }
                break;
            case DroneState.Approach:
                {
                    Vector3 targetPosition = Vector3.MoveTowards(transform.position, approachTarget.position, moveSpeed * deltaTime);
                    Vector3 lookDir = approachTarget.position - transform.position;
                    Quaternion targetRot;
                    if (lookDir != Vector3.zero)
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), 10.0f * deltaTime);
                    else
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 10.0f * deltaTime);

                    transform.SetPositionAndRotation(targetPosition, targetRot);

                    if ((transform.position - approachTarget.position).sqrMagnitude < 1f)
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
                            approachTarget = orderList.Pop();
                        }
                    }
                }
                break;
            case DroneState.Return:
                {
                    Vector3 destination = (target.forward * forwardOffset + target.right * rightOffset + target.up * upOffset) + target.position;
                    Vector3 targetPosition = Vector3.MoveTowards(transform.position, destination, deltaTime * moveSpeed);
                    Vector3 lookDir = targetPosition - transform.position;
                    Quaternion targetRot;
                    if (lookDir != Vector3.zero)
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), 10.0f * deltaTime);
                    else
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 10.0f * deltaTime);

                    transform.SetPositionAndRotation(targetPosition, targetRot);

                    if ((transform.position - destination).sqrMagnitude <=2.0f)
                    {
                        state = DroneState.Default;
                    }
                }
                break;
        }
    }

    public void OrderApproch(Transform target)
    {
        if (state == DroneState.Default || state == DroneState.Return)
        {
            state = DroneState.Approach;
            approachTarget = target;
        }
        else
        {
            orderList.Push(target);
        }
    }
}
