using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearCtrl : MonoBehaviour
{
    [SerializeField]private LayerMask enterLayer;
    private Rigidbody rigidbody;
    private bool isOver = false;
    private Vector3 launchPos;
    [SerializeField]private Transform hangingPoint;
    [SerializeField]private RopeBuiltIn ropePrefab;

    private RopeBuiltIn rope;
    private bool isBounced = false;
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        rope = Instantiate(ropePrefab,hangingPoint.position,Quaternion.identity);
        rope.spearBody = rigidbody;
        rope.SetAnchorPos(hangingPoint.position);
        
    }

    public void Update()
    {
        rope.SetAnchorPos(hangingPoint.position);
    }

    public void Launch(Vector3 launchPos)
    {
        this.launchPos = launchPos;
        rigidbody.AddForce(transform.forward * 50.0f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isOver == false)
        {
            //Debug.Log(enterLayer.value);
            if((1 << collision.gameObject.layer & enterLayer.value) == 0)
            {
                rigidbody.velocity *= 0.4f;
                isBounced = true;
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.Play("SpearOut", transform.position).audioObject.volume = rigidbody.velocity.sqrMagnitude * 0.01f;
                }
                return;
            }

            if(isBounced)
                return;

            
            rigidbody.velocity = Vector3.zero;
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            //Destroy(rigidbody);
            var rot = transform.rotation;
            transform.parent = collision.transform;
            transform.rotation = Quaternion.LookRotation(-collision.contacts[0].normal,Vector3.up);
            isOver = true;

            rope.canHanging = true;
            if (AudioManager.instance != null)
            {
                AudioManager.instance.Play("SpearIn", transform.position).audioObject.volume = rigidbody.velocity.sqrMagnitude * 0.01f;
            }
            //RopeBuiltIn newRope = Instantiate(ropePrefab, hangingPoint.position, Quaternion.identity);
            //newRope.InitializeRope(launchPos);
        }
    }

    public void OnDisable()
    {
        if(rope != null)
        {
            rope.Disable();
        }
    }
}
