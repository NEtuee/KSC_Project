using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowTargetCtrl : UnTransfromObjectBase
{

    public RectTransform aimTransform;
    public RectTransform supportTransform;
    public float crosshairMovingSpeed = 5f;
    public float aimLimitDist = 500f;
    public float aimMovingSpeed = 2.5f;
    public float spineRotateDivide = 1000f;

    [SerializeField] private bool visible = true;
    public bool Visible => visible;
    [SerializeField] private Transform target;
    [SerializeField] private float yawRotateSpeed = 120f;
    [SerializeField] private float pitchRotateSpeed = 120f;
    [SerializeField] private float pitchLimitMin = -80f;
    [SerializeField] private float pitchLimitMax = 50f;
    [SerializeField] private float rotSmooth = 0.1f;
    [SerializeField] private float followSmooth = 8f;
    [SerializeField] private float revisionSpeed = 100.0f;
    [SerializeField] private bool isPause;

    [SerializeField]private PlayerUnit _player;

    public List<Transform> _gunTargetObjects = new List<Transform>();

    private float _mouseX;
    private float _mouseY;

    private bool _isAim = false;

    public float YawRotateSpeed
    {
        get => yawRotateSpeed;
        set => yawRotateSpeed = value;
    }

    public float PitchRotateSpeed
    {
        get => pitchRotateSpeed;
        set => pitchRotateSpeed = value;
    }

    [SerializeField]private Vector3 currentRot;
    [SerializeField]private Vector3 targetRot;

    private float currentYawRotVelocity;
    private float currentPitchRotVelocity;

    private Vector2 currentAimVelocity;
    private Vector2 supportVelocity;

    private Vector3 smoothVelocity;

    [SerializeField]private bool updateMode = false;

    public override void Assign()
    {
        base.Assign();

        SaveMyNumber("FollowTargetCtrl");

        AddAction(MessageTitles.set_setplayer, (msg) =>
        {
            _player = (PlayerUnit)msg.data;
            target = _player.transform;
        });

        AddAction(MessageTitles.set_gunTargetMessageObject, (msg) =>
        {
            _gunTargetObjects.Add((Transform)msg.data);
        });
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("PlayerManager"));

        currentRot = transform.localRotation.eulerAngles;
        targetRot = currentRot;

        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);

    }
    public static float Clamp0360(float eulerAngles)
    {
        float result = eulerAngles - Mathf.Ceil(eulerAngles / 360f) * 360f;
        if (result < 0)
        {
            result += 360f;
        }
        return result;
    }

    private void FixedUpdate()
    {
        if (_player.GetState == PlayerUnit.deadState)
            return;

        if (!visible)
            return;

        transform.position = target.position + Vector3.up;


        targetRot.y += _mouseX * yawRotateSpeed * Time.fixedDeltaTime;
        targetRot.y = Clamp0360(targetRot.y);
        targetRot.x += _mouseY * pitchRotateSpeed * Time.fixedDeltaTime;

        if(_isAim)
        {
            var target = targetRot;
            target.x = target.y;
            target.y = -targetRot.x;
            aimTransform.anchoredPosition = Vector2.SmoothDamp(aimTransform.anchoredPosition,target * crosshairMovingSpeed,
                                                            ref currentAimVelocity,rotSmooth);

            if (_player.GamepadMode == true)
            {
                bool detect = false;
                Vector2 nearestGunTarget = new Vector2();
                Vector2 aimTransformPos = aimTransform.transform.position;
                for (int i = 0; i < _gunTargetObjects.Count; i++)
                {
                    if (_gunTargetObjects[i] == null)
                    {
                        _gunTargetObjects.RemoveAt(i);
                        continue;
                    }

                    if (_gunTargetObjects[i].gameObject.activeInHierarchy == false)
                    {
                        ++i;
                        continue;
                    }

                    if (Vector3.Dot((_gunTargetObjects[i].position - Camera.main.transform.position).normalized, Camera.main.transform.forward) > 0.0f)
                    {
                        Vector2 screenPos = Camera.main.WorldToScreenPoint(_gunTargetObjects[i].position);
                        if (detect == false)
                        {
                            nearestGunTarget = screenPos;
                        }
                        else if ((aimTransformPos - nearestGunTarget).sqrMagnitude > (aimTransformPos - screenPos).sqrMagnitude)
                        {
                            nearestGunTarget = screenPos;
                        }
                        detect = true;
                    }
                }

                supportTransform.position = nearestGunTarget;
                if (detect == true && (aimTransformPos - nearestGunTarget).magnitude <= 500.0f)
                {
                    //aimTransform.transform.position = Vector2.SmoothDamp(aimTransform.transform.position, nearestGunTarget, ref supportVelocity, 1f);
                    aimTransform.transform.position = Vector2.MoveTowards(aimTransform.transform.position, nearestGunTarget, 300.0f * Time.fixedDeltaTime);
                }
            }


            if (aimTransform.anchoredPosition.magnitude > aimLimitDist)
            {
                var dir = aimTransform.anchoredPosition.normalized;
                var camFactor = aimTransform.anchoredPosition - (dir * aimLimitDist);
                aimTransform.anchoredPosition -= camFactor;
                
                targetRot.x = -aimTransform.anchoredPosition.y;
                targetRot.y = aimTransform.anchoredPosition.x;
                targetRot /= crosshairMovingSpeed;

                var x = camFactor.x;
                camFactor.x = -camFactor.y;
                camFactor.y = x;

                var euler = transform.eulerAngles;
                euler.x = ToWrapAngle(euler.x);

                var rot = (Vector3)(camFactor * crosshairMovingSpeed * aimMovingSpeed * Time.fixedDeltaTime) + euler;
                rot.x = Mathf.Clamp(rot.x, pitchLimitMin, pitchLimitMax);
                Quaternion rotation = Quaternion.Euler(rot.x, rot.y, 0f);
                transform.rotation = rotation;
            }

            var spineRot = aimTransform.anchoredPosition / spineRotateDivide;
            spineRot.y *= -1f;
            MathEx.Swap<float>(ref spineRot.x, ref spineRot.y);
            
            var data = MessageDataPooling.GetMessageData<MD.Vector3Data>();
            data.value = spineRot;
            SendMessageEx(MessageTitles.playermanager_setSpineRotation,GetSavedNumber("PlayerManager"),data);
        }
        else
        {
            if (_mouseX == 0.0f && _mouseY == 0.0f &&
                Mathf.Abs(_player.InputHorizontal) > 0.6f
                && _player.GetState == PlayerUnit.hangLedgeState 
                && _player.climbDir != ClimbDir.Stop)
            {
                Vector3 look = new Vector3();
                if (_player.climbDir == ClimbDir.Left)
                {
                    look = _player.Transform.forward - _player.Transform.right;
                }
                else if (_player.climbDir == ClimbDir.Right)
                {
                    look = _player.Transform.forward + _player.Transform.right;
                }

                Vector3 target = Quaternion.LookRotation(look).eulerAngles;
                target.x = Mathf.Clamp(targetRot.x, pitchLimitMin, pitchLimitMax);
                targetRot.x = Mathf.SmoothDamp(targetRot.x, target.x, ref currentPitchRotVelocity, revisionSpeed);
                targetRot.y = Mathf.SmoothDamp(targetRot.y, target.y, ref currentYawRotVelocity, revisionSpeed);
                Quaternion localRotation = Quaternion.Euler(targetRot.x, targetRot.y, 0.0f);
                transform.rotation = localRotation;
            }
            else
            {
                targetRot.x = Mathf.Clamp(targetRot.x, pitchLimitMin, pitchLimitMax);
                currentRot.x = Mathf.SmoothDamp(currentRot.x, targetRot.x, ref currentPitchRotVelocity, rotSmooth);
                currentRot.y = Mathf.SmoothDamp(currentRot.y, targetRot.y, ref currentYawRotVelocity, rotSmooth);
              
                Quaternion localRotation = Quaternion.Euler(targetRot.x, targetRot.y, 0.0f);
                transform.rotation = localRotation;
            }
        }

    }

    private void LateUpdate()
    {
        //if (GameManager.Instance.PAUSE == true)
        //    return;

        //if (updateMode == false)
        //    return;

        //transform.position = Vector3.Lerp(transform.position, target.position + Vector3.up, followSmooth * Time.deltaTime);
        //transform.position = target.position + Vector3.up + Vector3.up * ((PlayerCtrl_Ver2)GameManager.Instance.player).GetFootStepOffset() * 0.1f;
        //transform.position = target.position + Vector3.up;
        //transform.position = Vector3.Lerp(transform.position, target.position + Vector3.up, 5.0f * Time.deltaTime);
        //transform.position = Vector3.MoveTowards(transform.position, target.position + Vector3.up, 8.0f * Time.deltaTime);
    }

    public void Pause(){ isPause = true; }
    public void Resume() { isPause = false; }

    public void SetForceRotation(Vector3 rot)
    {
        currentRot = rot;
        targetRot = rot;
    }

    public void SetYawRotateSpeed(float speed)
    {
        yawRotateSpeed = speed;
    }

    public void SetPitchRotateSpeed(float speed)
    {
        pitchRotateSpeed = speed;
    }

    public void SetPitchYaw(float pitch, float yaw)
    {
        transform.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
        currentRot = transform.localRotation.eulerAngles;
        targetRot = currentRot;
    }

    public void SetPitchYawPosition(float pitch, float yaw, Vector3 position)
    {
        transform.rotation = Quaternion.Euler(pitch, yaw, 0.0f);
        currentRot = transform.localRotation.eulerAngles;
        targetRot = currentRot;
        transform.position = position;
    }

    public void OnCamera(InputAction.CallbackContext value)
    {
        if (Time.timeScale == 0f)
            return;

        Vector2 inputVector = value.ReadValue<Vector2>();
        _mouseY = inputVector.y;
        _mouseX = inputVector.x;
    }

    public void SetAim(bool active)
    {
        _isAim = active;

        if(_isAim)
        {
            aimTransform.anchoredPosition = Vector2.zero;
            currentRot = Vector3.zero;
        }
        else
        {
            currentRot = transform.rotation.eulerAngles;

            currentRot.x = ToWrapAngle(currentRot.x);

            currentPitchRotVelocity = 0f;
            currentYawRotVelocity = 0f;
        }
        
        targetRot = currentRot;
    }

    public float ToWrapAngle(float angle)
    {
        angle %= 360;
        if(angle >180)
            angle -= 360;
        
        return angle;
    }
}
