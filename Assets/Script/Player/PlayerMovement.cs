using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Vector3 velocity;
    [SerializeField] private float speed;
    [SerializeField] private float trueSpeed;
    [SerializeField] private float groundDistance;

    [Header("Ground")]
    public float groundMinDistance = 0.1f;
    public float groundMaxDistance = 0.5f;
    public LayerMask groundLayer;
    public bool isGrounded;

    private Vector3 prevPosition;

    private CapsuleCollider capsuleCollider;
    private float colliderHeight;

    private RaycastHit groundHit;

    public Vector3 Velocity { get { return velocity; } protected set { velocity = value; } }
    public float Speed { get { return speed; } protected set { speed = value; } }
    public float TrueSpeed { get { return trueSpeed; } protected set { trueSpeed = value; } }

    private void Start()
    {
        prevPosition = transform.position;
        velocity = Vector3.zero;
        speed = 0.0f;
        trueSpeed = 0.0f;

        capsuleCollider = GetComponent<CapsuleCollider>();
        if(capsuleCollider != null)
        {
            colliderHeight = capsuleCollider.height;
        }
    }

    public void Move(Vector3 direction)
    {
        transform.position += direction * Time.deltaTime;
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
                }
            }
            groundDistance = (float)System.Math.Round(dist, 2);
        }
    }

    private void CheckGround()
    {
        CheckGroundDistance();

        if(groundDistance <= groundMinDistance)
        {
            isGrounded = true;
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
