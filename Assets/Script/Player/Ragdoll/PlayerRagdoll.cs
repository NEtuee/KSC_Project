using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRagdoll : MonoBehaviour
{
    private Animator anim;
    private Collider collider;
    [SerializeField]private List<Rigidbody> ragdollRigids = new List<Rigidbody>();

    void Start()
    {
        anim = GetComponent<Animator>();
        collider = GetComponent<Collider>();

        Rigidbody[] rigidBodies = transform.Find("Bip001").GetComponentsInChildren<Rigidbody>();

        foreach(Rigidbody rigid in rigidBodies)
        {
            if(rigid.transform == transform)
            {
                continue;
            }

            ragdollRigids.Add(rigid);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            ActiveRagdoll(true);
        }
    }

    private void ActiveRagdoll(bool active)
    {
        anim.enabled = !active;
        collider.enabled = !active;

        foreach(Rigidbody rigid in ragdollRigids)
        {
            Collider collider = rigid.transform.GetComponent<Collider>();
            collider.isTrigger = !active;
        }
    }
}
