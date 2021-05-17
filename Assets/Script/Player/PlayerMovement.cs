using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool movementDebug;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float speed;
    [SerializeField] private float trueSpeed;
    public LayerMask fowardCheckLayer;

    private UpdateMethod updateMethod;

    [Header("Ground")]
    [SerializeField] private float groundDistance;
    public float groundMinDistance = 0.1f;
    public float groundMaxDistance = 0.5f;
    public float groundSlopMinDistanc = 0.6f;
    public LayerMask groundLayer;
    public bool isGrounded;
    public bool isJumping;
    public float jumpMinTime = 0.5f;
    private float jumpTime;

    [SerializeField]private Transform prevParent;
    [SerializeField]private float detachTime;
    private float speedKeepTime = 5f;
    private Vector3 prevParentPrevPos;
    [SerializeField]private bool keepSpeed;
    [SerializeField] private float amount;

    [Header("Slide")]
    [SerializeField] private float groundAngle = 0.0f;
    [SerializeField] private float invalidityAngle = 70.0f;
    [SerializeField] private float slideAngle = 50.0f;
    [SerializeField] private float currentJumpPower = 0.0f;
    [SerializeField] private float minJumpPower = -10.0f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float gravity = 20.0f;
    private Vector3 slidingVector = Vector3.zero;

    private Vector3 prevPosition;

    private CapsuleCollider capsuleCollider;
    private float colliderHeight;

    private RaycastHit groundHit;
    private Transform detectObject;
    private Rigidbody rigidbody;
    private PlayerRagdoll ragdoll;
    private PlayerCtrl_Ver2 player;
    private CapsuleCollider collider;

    [SerializeField]private float slidingTime = 0.0f;
    [SerializeField] private float holdTime = 1f;

    public Vector3 Velocity { get { return velocity; } protected set { velocity = value; } }
    public float Speed { get { return speed; } protected set { speed = value; } }
    public float TrueSpeed { get { return trueSpeed; } protected set { trueSpeed = value; } }

    private void Start()
    {
        prevPosition = transform.position;
        velocity = Vector3.zero;
        speed = 0.0f;
        trueSpeed = 0.0f;
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        player = GetComponent<PlayerCtrl_Ver2>();
        if(capsuleCollider != null)
        {
            colliderHeight = capsuleCollider.height;
        }

        ragdoll = GetComponent<PlayerRagdoll>();
        collider = GetComponent<CapsuleCollider>();

        updateMethod = player.updateMethod == UpdateMethod.Update ? UpdateMethod.Update : UpdateMethod.FixedUpdate;
    }

    public void Move(Vector3 direction)
    {
        //Vector3 p1 = transform.position + capsuleCollider.center + Vector3.up * -capsuleCollider.height * 0.5f;
        //Vector3 p2 = p1 + Vector3.up * capsuleCollider.height;

        //if (Physics.CapsuleCast(p1, p2, capsuleCollider.radius * 1.5f, transform.forward, 0.0f, fowardCheckLayer))
        //    return;

        //rigidbody.MovePosition(transform.position+direction * Time.fixedDeltaTime);
        //rigidbody.position = transform.position + direction * Time.fixedDeltaTime;

        float deltaTime = player.updateMethod == UpdateMethod.FixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;
        transform.position += direction * deltaTime;
    }

    public void Move_Nodelta(Vector3 direction)
    {
        if (Physics.Raycast(transform.position + collider.center, direction, collider.radius + direction.magnitude) == true)
        {
            return;
        }

        transform.position += direction;
    }


    public void Jump()
    {
        isJumping = true;
        //isGrounded = false;
        SetGrounded(false);
        jumpTime = Time.time;

        keepSpeed = true;
        prevParent = transform.parent;
        detachTime = Time.time;
        if (prevParent != null)
        {
            prevParentPrevPos = prevParent.position;
            keepSpeed = true;
        }

        SetParent(null);
    }
    
    private void Update()
    {
        if (GameManager.Instance.PAUSE == true)
            return;

        if(groundAngle >= invalidityAngle)
        {
            currentJumpPower -= gravity * Time.deltaTime;
            currentJumpPower = Mathf.Clamp(currentJumpPower, minJumpPower, 50f);

            //transform.position += slidingVector * currentJumpPower * Time.deltaTime;
        }
        else
        {
            currentJumpPower = 0.0f;
        }

        if (updateMethod == UpdateMethod.Update)
        {
            MoveConservation();
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.PAUSE == true)
            return;

        velocity = transform.position - prevPosition;
        trueSpeed = velocity.magnitude;
        speed = trueSpeed * 100f;
        prevPosition = transform.position;

        if (player.GetState() == PlayerCtrl_Ver2.PlayerState.Grab || 
            player.GetState() == PlayerCtrl_Ver2.PlayerState.LedgeUp ||
            player.GetState() == PlayerCtrl_Ver2.PlayerState.Ragdoll || 
            player.GetState() == PlayerCtrl_Ver2.PlayerState.HangRagdoll ||
            player.GetState() == PlayerCtrl_Ver2.PlayerState.HangLedge ||
            player.GetState() == PlayerCtrl_Ver2.PlayerState.LedgeUp ||
            player.GetState() == PlayerCtrl_Ver2.PlayerState.HangShake)
        {
            groundDistance = 0.0f;
            return;
        }

        CheckGround();

        if (player.GetState() == PlayerCtrl_Ver2.PlayerState.Default)
        {
            if (groundAngle >= slideAngle && groundAngle < invalidityAngle)
            {
                slidingTime += Time.fixedDeltaTime;
            }
            else
            {
                slidingTime = 0;
            }

            if (slidingTime > holdTime)
            {
                slidingTime = 0.0f;
                //ragdoll.SlidingRagdoll(-slidingVector*30f);
            }
        }
        else
        {
            slidingTime = 0f;
        }

        if(updateMethod == UpdateMethod.FixedUpdate)
        {
            MoveConservation();
        }
    }

    private void MoveConservation()
    {
        if (keepSpeed == true && prevParent != null)
        {
            amount = 1 - Mathf.InverseLerp(detachTime, detachTime + speedKeepTime, Time.time);

            Vector3 difference = (prevParent.position - prevParentPrevPos);
            float velocity = difference.magnitude;
            difference.y = 0;
            difference.Normalize();
            prevParentPrevPos = prevParent.position;
            Move_Nodelta(difference * velocity * amount);

            if (Mathf.Abs(amount) < Mathf.Epsilon)
            {
                keepSpeed = false;
            }
        }
    }


    private void CheckGroundDistance()
    {
        if (capsuleCollider != null)
        {
            float radius = capsuleCollider.radius * 0.9f;
            float dist = 10f;

            Ray ray2 = new Ray(transform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);
            if (Physics.Raycast(ray2, out groundHit, (colliderHeight / 2) + dist, groundLayer) && !groundHit.collider.isTrigger)
            {
                dist = transform.position.y - groundHit.point.y;

                detectObject = groundHit.collider.transform;
                groundAngle = Mathf.Acos(Vector3.Dot(groundHit.normal, Vector3.up)) * Mathf.Rad2Deg;
                slidingVector = (Vector3.Project(Vector3.down, groundHit.normal) - Vector3.down).normalized;
            }
            else
            {
                detectObject = null;
            }

            if (dist >= groundMinDistance)
            {
                Vector3 pos = transform.position + Vector3.up * (capsuleCollider.radius);
                Ray ray = new Ray(pos, -Vector3.up);
                if (Physics.SphereCast(ray, radius, out groundHit, capsuleCollider.radius + groundMaxDistance, groundLayer) && !groundHit.collider.isTrigger)
                {
                    Physics.Linecast(groundHit.point + (Vector3.up * 0.1f), groundHit.point + Vector3.down * 0.15f, out groundHit, groundLayer);
                    float newDist = transform.position.y - groundHit.point.y;
                    if (dist > newDist)
                    {
                        dist = newDist;
                    }

                    groundAngle = Mathf.Acos(Vector3.Dot(groundHit.normal, Vector3.up)) * Mathf.Rad2Deg;
                    slidingVector = (Vector3.Project(Vector3.down, groundHit.normal)- Vector3.down).normalized;
                }
            }
            groundDistance = (float)System.Math.Round(dist, 2);
            if(float.IsNaN(groundAngle) == true)
            {
                groundAngle = 0.0f;
            }
        }
    }


    private void CheckGround()
    {
        if(isJumping == true &&(Time.time - jumpTime < jumpMinTime))
        {
            return;
        }

        CheckGroundDistance();

        if (groundDistance <= groundMinDistance)
        {
            if (groundAngle < invalidityAngle)
            {
                //isGrounded = true;
                SetGrounded(true);
                isJumping = false;

                if (detectObject != null && detectObject.CompareTag("Enviroment"))
                {
                    SetParent(detectObject);
                }
                else
                {
                    if (player.GetState() != PlayerCtrl_Ver2.PlayerState.Grab
                        && player.GetState() != PlayerCtrl_Ver2.PlayerState.LedgeUp
                        && player.GetState() != PlayerCtrl_Ver2.PlayerState.Ragdoll
                        && player.GetState() != PlayerCtrl_Ver2.PlayerState.HangRagdoll
                        && player.GetState() != PlayerCtrl_Ver2.PlayerState.HangLedge
                        && player.GetState() != PlayerCtrl_Ver2.PlayerState.LedgeUp
                        && player.GetState() != PlayerCtrl_Ver2.PlayerState.HangShake)
                    {
                        SetParent(null);
                    }
                }

                keepSpeed = false;
            }
            else
            {
                //isGrounded = false;
                SetGrounded(false);
            }
            
        }
        else
        {
            if(groundDistance >= groundMaxDistance)
            {
                if(isGrounded == true)
                {
                    prevParent = transform.parent;
                    detachTime = Time.time;

                    if (prevParent != null)
                    {
                        prevParentPrevPos = prevParent.position;
                        keepSpeed = true;
                    }


                    if (player.GetState() == PlayerCtrl_Ver2.PlayerState.Grab)
                    {
                        keepSpeed = false;
                    }
                }

                //isGrounded = false;
                SetGrounded(false);
                if (player.GetState() != PlayerCtrl_Ver2.PlayerState.Grab &&
                    player.GetState() != PlayerCtrl_Ver2.PlayerState.LedgeUp && 
                    player.GetState() != PlayerCtrl_Ver2.PlayerState.Ragdoll && 
                    player.GetState() != PlayerCtrl_Ver2.PlayerState.HangRagdoll&&
                    player.GetState() != PlayerCtrl_Ver2.PlayerState.HangLedge&&
                    player.GetState() != PlayerCtrl_Ver2.PlayerState.LedgeUp && 
                    player.GetState() != PlayerCtrl_Ver2.PlayerState.ReadyClimbingJump &&
                    player.GetState() != PlayerCtrl_Ver2.PlayerState.ClimbingJump && 
                    player.GetState() != PlayerCtrl_Ver2.PlayerState.HangShake&&
                    player.GetState() != PlayerCtrl_Ver2.PlayerState.ReadyGrab)
                {
                    SetParent(null);
                }
            }
        }
    }

    public void Detach()
    {
        if (player.GetState() == PlayerCtrl_Ver2.PlayerState.Grab)
        {
            keepSpeed = false;
            return;
        }

        prevParent = transform.parent;
        SetParent(null);
        detachTime = Time.time;

        if (prevParent != null)
        {
            prevParentPrevPos = prevParent.position;
            keepSpeed = true;
        }
    }

    public void Attach()
    {
        keepSpeed = false;
        //isGrounded = false;
        SetGrounded(false);
    }

    public float GetGroundAngle()
    {
        return groundAngle;
    }

    public void SetParent(Transform parent)
    {
        if (transform.parent != parent)
        {
            transform.SetParent(parent);
        }
    }

    public void SetGrab()
    {
        isJumping = false;
    }

    public void SetGrounded(bool value)
    {
        isGrounded = value;
        if(value == false)
        {
            Debug.Log("Set IsGrounded False" + " Current Ground Angle"+groundAngle);
        }
    }

    private void OnGUI()
    {
        if (movementDebug == true)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;

            GUI.Label(new Rect(10f, 260f, 100, 20), "IsGround : " + isGrounded.ToString(), style);
            GUI.Label(new Rect(10f, 280f, 100, 20), "GroundAngle : " + groundAngle.ToString(), style);
            GUI.Label(new Rect(10f, 300f, 100, 20), "IsKeepSpeed : " + keepSpeed.ToString(), style);
            GUI.Label(new Rect(10f, 480f, 100, 20), "GroundDistance : " + groundDistance.ToString(), style);

        }
    }

}
