using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float speed;
    [SerializeField] private float trueSpeed;
    public LayerMask fowardCheckLayer;

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

    [SerializeField]private float slidingTime = 0.0f;

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
    }

    public void Move(Vector3 direction)
    {
        //Vector3 p1 = transform.position + capsuleCollider.center + Vector3.up * -capsuleCollider.height * 0.5f;
        //Vector3 p2 = p1 + Vector3.up * capsuleCollider.height;

        //if (Physics.CapsuleCast(p1, p2, capsuleCollider.radius * 1.5f, transform.forward, 0.0f, fowardCheckLayer))
        //    return;

        transform.position += direction * Time.fixedDeltaTime;
        //rigidbody.MovePosition(transform.position+direction * Time.fixedDeltaTime);
        //rigidbody.position = transform.position + direction * Time.fixedDeltaTime;
    }

    public void Move_Nodelta(Vector3 direction)
    {
        transform.position += direction;
    }


    public void Jump()
    {
        isJumping = true;
        isGrounded = false;
        jumpTime = Time.time;

        keepSpeed = true;
        prevParent = transform.parent;
        detachTime = Time.time;
        if (prevParent != null)
        {
            prevParentPrevPos = prevParent.position;
            keepSpeed = true;
        }

        transform.SetParent(null);
    }

    private void Update()
    {
        if(isGrounded == true && groundAngle >= invalidityAngle)
        {
            currentJumpPower -= gravity * Time.deltaTime;
            currentJumpPower = Mathf.Clamp(currentJumpPower, minJumpPower, 50f);

            transform.position += slidingVector * currentJumpPower * Time.deltaTime;
        }
        else
        {
            currentJumpPower = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        velocity = transform.position - prevPosition;
        trueSpeed = velocity.magnitude;
        speed = trueSpeed * 100f;
        prevPosition = transform.position;

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

            if (slidingTime > 3.0f)
            {
                slidingTime = 0.0f;
                ragdoll.SlidingRagdoll(-slidingVector*30f);
            }
        }
        else
        {
            slidingTime = 0f;
        }

        if(keepSpeed == true && prevParent != null)
        {
            amount = 1 -Mathf.InverseLerp(detachTime, detachTime + speedKeepTime, Time.time);

            Vector3 difference = (prevParent.position - prevParentPrevPos);
            float velocity = difference.magnitude;
            difference.y = 0;
            difference.Normalize();
            prevParentPrevPos = prevParent.position;
            Move_Nodelta(difference * velocity * amount);

            if(Mathf.Abs(amount) < Mathf.Epsilon)
            {
                keepSpeed = false;
            }
        }
    }

    private void CheckGroundDistance()
    {
        if(capsuleCollider != null)
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
        }
    }


    private void CheckGround()
    {
        if(isJumping == true &&(Time.time - jumpTime < jumpMinTime))
        {
            return;
        }

        CheckGroundDistance();

        if(groundDistance <= groundMinDistance)
        {
            isGrounded = true;
            isJumping = false;

            if (!detectObject.CompareTag("Env_Props"))
            {
                transform.SetParent(detectObject);
            }

            keepSpeed = false;
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

                isGrounded = false;
                if (player.GetState() != PlayerCtrl_Ver2.PlayerState.Grab && player.GetState() != PlayerCtrl_Ver2.PlayerState.LedgeUp && player.GetState() != PlayerCtrl_Ver2.PlayerState.Ragdoll)
                {
                    transform.SetParent(null);
                }
            }
        }
    }
}
