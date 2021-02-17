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

    [Header("Slide")]
    [SerializeField] private float groundAngle = 0.0f;
    [SerializeField] private float currentJumpPower = 0.0f;
    [SerializeField] private float minJumpPower = -10.0f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float gravity = 20.0f;
    private Vector3 slidingVector = Vector3.zero;

    private Vector3 prevPosition;

    private CapsuleCollider capsuleCollider;
    private float colliderHeight;

    private RaycastHit groundHit;
    private Rigidbody rigidbody;

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
        if(capsuleCollider != null)
        {
            colliderHeight = capsuleCollider.height;
        }
    }

    public void Move(Vector3 direction)
    {
        Vector3 p1 = transform.position + capsuleCollider.center + Vector3.up * -capsuleCollider.height * 0.5f;
        Vector3 p2 = p1 + Vector3.up * capsuleCollider.height;

        if (Physics.CapsuleCast(p1, p2, capsuleCollider.radius * 1.5f, transform.forward, 0.0f, fowardCheckLayer))
            return;

        transform.position += direction * Time.fixedDeltaTime;
        //rigidbody.MovePosition(transform.position+direction * Time.fixedDeltaTime);
        //rigidbody.position = transform.position + direction * Time.fixedDeltaTime;
    }

    public void Jump()
    {
        isJumping = true;
        isGrounded = false;
        jumpTime = Time.time;
    }

    private void Update()
    {
        if(isGrounded == true && groundAngle >= 70f)
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
        }
        else
        {
            if(groundDistance >= groundMaxDistance)
            {
                isGrounded = false;
            }
        }
    }
}
