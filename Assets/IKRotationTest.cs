using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKRotationTest : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private Vector3 targetrot;
    [SerializeField] private Vector3 targetPos;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1.0f);
        anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.Euler(targetrot));
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1.0f);
        anim.SetIKPosition(AvatarIKGoal.LeftFoot, targetPos);
    }
}
