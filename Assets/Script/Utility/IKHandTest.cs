using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandTest : MonoBehaviour
{
    public Transform target;
    public Animator animator;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKPosition(AvatarIKGoal.LeftHand, target.position);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
    }
}
