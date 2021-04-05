using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public enum DroneState { Default, Approach, Collect, Return , AimHelp ,Help}

    [SerializeField] private Transform target;
    [SerializeField] private DroneState state;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Vector3 defaultFollowOffset;
    [SerializeField] private Vector3 aimHelpOffset;
    [SerializeField] private Vector3 helpOffset;
    [SerializeField] private Vector3 helpGrabStateOffset;
    [SerializeField] private float collectRequiredTime = 1f;
    [SerializeField] private FloatingMove floatingMove;
    [SerializeField] private bool help = false;
    private float collectStartTime;
    
    private Transform approachTarget;
    private Stack<Transform> orderList = new Stack<Transform>();
    private Transform mainCam;

    private Transform playerHead;
    private PlayerCtrl_Ver2 player;

    private DroneHelperRoot droneHelperRoot;

    public delegate void WhenAimHelp();
    public WhenAimHelp whenAimHelp;
    public delegate void WhenHelp();
    public WhenAimHelp whenHelp;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main.transform;
        playerHead = GameManager.Instance.player.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
        player = (PlayerCtrl_Ver2)GameManager.Instance.player;
        droneHelperRoot = GetComponent<DroneHelperRoot>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance.PAUSE == true)
            return;

        if (((PlayerCtrl_Ver2)GameManager.Instance.player).updateMethod != UpdateMethod.FixedUpdate)
            return;

        UpdateDrone(Time.fixedDeltaTime);
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.PAUSE == true)
            return;

        if (Input.GetKeyDown(KeyCode.N))
        {
            OrderHelp();
        }

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
                    Vector3 targetPosition = (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
                    targetPosition = Vector3.Lerp(transform.position, targetPosition, deltaTime * 5f);
                    Vector3 lookDir = targetPosition - transform.position;
                    lookDir.y = 0.0f;
                    Quaternion targetRot;
                    if (lookDir != Vector3.zero)
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), 10.0f * deltaTime);
                    else
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 10.0f * deltaTime);

                    transform.SetPositionAndRotation(targetPosition, targetRot);
                }
                break;
            case DroneState.AimHelp:
                {
                    Vector3 targetPosition = (target.forward * aimHelpOffset.z + target.right * aimHelpOffset.x + target.up * aimHelpOffset.y) + target.position;
                    targetPosition = Vector3.Lerp(transform.position, targetPosition, deltaTime * 12f);
                    Vector3 lookDir = playerHead.position - transform.position;
                    Quaternion targetRot;
                    if (lookDir != Vector3.zero)
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir, Vector3.up), 10.0f * deltaTime);
                    else
                        targetRot = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.up), 10.0f * deltaTime);

                    transform.SetPositionAndRotation(targetPosition, targetRot);
                }
                break;
            case DroneState.Help:
                {
                    Vector3 camForward = Camera.main.transform.forward;
                    Vector3 camRight = Camera.main.transform.right;
                    camForward.y = 0;
                    camRight.y = 0;


                    Vector3 targetPosition; 
                    if(player.GetState() != PlayerCtrl_Ver2.PlayerState.Grab)
                        targetPosition= (camForward * helpOffset.z + camRight * helpOffset.x + Vector3.up * helpOffset.y) + target.position;
                    else
                        targetPosition = (target.forward * helpGrabStateOffset.z + target.right * helpGrabStateOffset.x + Vector3.up * helpGrabStateOffset.y) + target.position;

                    targetPosition = Vector3.Lerp(transform.position, targetPosition, deltaTime * 5f);
                    Vector3 lookDir = playerHead.position - transform.position;
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
                    Vector3 destination = (target.forward * defaultFollowOffset.z + target.right * defaultFollowOffset.x + target.up * defaultFollowOffset.y) + target.position;
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

    public void OrderAimHelp(bool value)
    {
        if (value == true)
        {
            state = DroneState.AimHelp;
            floatingMove.SetRangeRatio(0.2f);
            whenAimHelp?.Invoke();
        }
        else
        {
            if(help == true)
            {
                state = DroneState.Help;
                floatingMove.SetRangeRatio(1.0f);
                whenHelp?.Invoke();
            }
            else
            {
                state = DroneState.Default;
                floatingMove.SetRangeRatio(1.0f);
            }
        }
    }

    public void OrderDefault()
    {
        help = false;
        if (state != DroneState.AimHelp)
        {
            state = DroneState.Default;
            floatingMove.SetRangeRatio(1.0f);
        }
    }

    public void OrderHelp()
    {
        help = true;

        if(state != DroneState.AimHelp)
        {
            state = DroneState.Help;
            whenHelp?.Invoke();
        }
    }

    public DroneState GetState() { return state; }

    public void DroneHelpCall(string key)
    {
        if(droneHelperRoot != null)
        {
            droneHelperRoot.HelpEvent(key);
        }
    }
}
