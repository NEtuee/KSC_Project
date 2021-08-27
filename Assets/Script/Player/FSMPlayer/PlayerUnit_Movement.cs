using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClimbingJumpDirection
{
    Up = 0,
    Left,
    Right,
    UpLeft,
    UpRight,
    Falling
}

public partial class PlayerUnit
{
    [Header("Movement")]
    [SerializeField] private bool keepSpeed;
    [SerializeField] private float amount;
    [SerializeField] private Transform prevParent;
    [SerializeField] private float detachTime;
    [SerializeField] private float speedKeepTime = 5f;
    private Vector3 prevParentPrevPos;


    private Transform detectObject;

    private void MoveConservation()
    {
        if(keepSpeed == true && prevParent != null)
        {
            amount = 1 - Mathf.InverseLerp(detachTime, detachTime + speedKeepTime, Time.time);

            Vector3 difference = (prevParent.position - prevParentPrevPos);
            float velocity = difference.magnitude;
            difference.y = 0;
            difference.Normalize();
            prevParentPrevPos = prevParent.position;
            Move(velocity * amount * difference, 0.0f, true);

            if(Mathf.Abs(amount) < Mathf.Epsilon)
            {
                keepSpeed = false;
            }
        }
    }

    public void Attach()
    {
        keepSpeed = false;
        isGrounded = false;
    }

    public void Detach()
    {
        if(_currentState == grabState)
        {
            keepSpeed = false;
            return;
        }

        prevParent = transform.parent;
        transform.parent = null;
        detachTime = Time.time;

        if (prevParent != null)
        {
            prevParentPrevPos = prevParent.position;
            keepSpeed = true;
        }
    }
    public void AdjustLedgeOffset()
    {
        Vector3 start = transform.position + transform.up * (_capsuleCollider.height * 2);
        RaycastHit upHit;
        Vector3 finalPosition;
        if (Physics.SphereCast(start, _capsuleCollider.radius * 2f, -transform.up, out upHit, _capsuleCollider.height * 2,
            adjustAbleLayer))
        {
            finalPosition = upHit.point + (transform.up * detectionOffset.y);
            finalPosition += transform.forward * detectionOffset.z;
            transform.position = finalPosition;

            StartCoroutine(ForceSnap(0.5f, finalPosition, transform.localPosition));
            //Debug.Log("AdjustLedgeOffset");
        }
    }

    IEnumerator ForceSnap(float snapTime, Vector3 snapPosition, Vector3 localPostiion)
    {
        float time = 0.0f;
        while (time < snapTime)
        {
            if (_currentState != hangLedgeState)
                break;

            //Debug.Log("Adjusting");
            //transform.position = snapPosition;

            if (transform.parent == null)
            {
                transform.position = snapPosition;
            }
            else
            {
                transform.localPosition = localPostiion;
            }

            time += Time.deltaTime;
            yield return null;
        }
    }

    public void UpdateClimbingInput()
    {
        if (isClimbingMove == true)
            return;

        if(_inputVertical == 0.0f)
        {
            climbingVertical = 0.0f;
        }
        else if(_inputVertical > 0.0f)
        {
            climbingVertical = 1.0f;
        }    
        else
        {
            climbingVertical = -1.0f;
        }

        if(_inputHorizontal > 0.0f)
        {
            climbingHorizon = Mathf.Ceil(_inputHorizontal);
        }
        else
        {
            climbingHorizon = Mathf.Floor(_inputHorizontal);
        }

        if(isLedge == false)
        {
            if(climbingVertical != 0.0f)
            {
                if(climbingVertical > 0.0f)
                {
                    if(climbingHorizon == 0.0f)
                    {
                        if(UpDetection() == false)
                        {
                            _animator.SetTrigger("UpClimbing");
                            isClimbingMove = true;
                        }
                    }
                    else
                    {
                        if (climbingHorizon > 0.0f)
                        {
                            _animator.SetTrigger("UpRightClimbing");
                            isClimbingMove = true;
                        }
                        else
                        {
                            _animator.SetTrigger("UpLeftClimbing");
                            isClimbingMove = true;
                        }
                    }
                }
                else
                {
                    if (climbingHorizon == 0.0f)
                    {
                        if (DownDetection() == true)
                        {
                            _animator.SetTrigger("DownClimbing");
                            isClimbingMove = true;
                        }
                    }
                    else
                    {
                        if (climbingHorizon > 0.0f)
                        {
                            _animator.SetTrigger("DownRightClimbing");
                            isClimbingMove = true;
                        }
                        else
                        {
                            _animator.SetTrigger("DownLeftClimbing");
                            isClimbingMove = true;
                        }
                    }
                }
            }
            else
            {
                if (climbingHorizon > 0.0f)
                {
                    _animator.SetTrigger("RightClimbing");
                    isClimbingMove = true;
                }
                else if (climbingHorizon < 0.0f)
                {
                    _animator.SetTrigger("LeftClimbing");
                    isClimbingMove = true;
                }
            }
        }
        else
        {
            if (climbingVertical == -1.0f)
            {
                if (_currentState != hangEdgeState)
                {
                    _animator.SetBool("IsLedge", false);
                    _animator.SetTrigger("DownClimbing");

                    isLedge = false;
                    isClimbingMove = true;
                    ChangeState(grabState);
                }
            }
            else if (climbingVertical == 0.0f && climbingHorizon != 0.0f)
            {
                if (climbingHorizon == 1.0f)
                {
                    _animator.SetTrigger("RightClimbing");
                    isClimbingMove = true;
                }
                else
                {
                    _animator.SetTrigger("LeftClimbing");
                    isClimbingMove = true;
                }
            }
        }
    }

    private bool UpDetection()
    {
        if (ledgeChecker.IsDetectedLedge() == true)
            return true;

        if (DetectionCanClimbingAreaByVertexColor(transform.position + transform.up * _capsuleCollider.height * 0.5f,
            transform.forward) == true)
        {
            return true;
        }

        if (DetectionCanClimbingAreaByVertexColor(transform.position + transform.up * _capsuleCollider.height * 0.75f,
            transform.forward) == true)
        {
            return true;
        }

        return false;
    }

    private bool DownDetection()
    {
        RaycastHit hit;
        Vector3 point1 = transform.position + transform.up * -0.3f;
        if (Physics.SphereCast(point1, _capsuleCollider.radius, transform.forward, out hit, 2f, detectionLayer))
        {
            return true;
        }

        return false;
    }

    public void ClimbingJump()
    {
        isGrounded = false;
        keepSpeed = true;
        prevParent = transform.parent;
        transform.parent = null;
        detachTime = Time.time;
        if(prevParent != null)
        {
            prevParentPrevPos = prevParent.position;
        }
    }
}
