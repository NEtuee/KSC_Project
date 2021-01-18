using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    //[SerializeField] private PlayerCtrl owner;
    [SerializeField] private PlayerCtrl_State owner;
    [SerializeField] private LayerMask groundLayer;
    private SphereCollider collider;
    [SerializeField]private bool isLock = false;
    private float detachTime = 1.5f;
    private float lockTime = 0.2f;
    [SerializeField] private float time;

    private IEnumerator detachCoroutine;
    private int detectLayer;
    private Vector3 originPos;

    private void Awake()
    {
        collider = GetComponent<SphereCollider>();
        detachCoroutine = DetachTimer();
        detectLayer = (1 << LayerMask.NameToLayer("ClimbingAble")) + (1 << LayerMask.NameToLayer("Floor"));
        originPos = transform.localPosition;
    }

    private void Update()
    {
        //RaycastHit hit;
        //if(Physics.SphereCast(transform.position,collider.radius,transform.forward,out hit,0.0f,detectLayer))
        //{
        //    owner.SetIsGround(true);
        //}
        //else
        //{
        //    owner.SetIsGround(false);
        //}
        transform.localPosition = originPos;
    }

    private void OnTriggerStay(Collider other)
    {
        if (isLock == true)
            return;

        if (other.CompareTag("Enviroment"))
        {
            owner.SetIsGround(true);
           
            if (owner.transform.parent != null)
            {
                if (other.gameObject != owner.transform.parent.gameObject)
                {
                    //owner.transform.parent = other.gameObject.transform;
                    owner.SetParent(other.gameObject.transform);
                }
            }
            else
            {
                //owner.transform.parent = other.gameObject.transform;
                owner.SetParent(other.gameObject.transform);
            }
        }
        else if(other.CompareTag("Env_Props"))
        {
            owner.SetIsGround(true);
            owner.SetParent(null);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enviroment"))
        {
            owner.SetIsGround(true);
            //owner.transform.parent = other.transform;
            if (owner.transform.parent != null)
            {
                if (other.gameObject != owner.transform.parent.gameObject)
                {
                    //owner.transform.parent = other.gameObject.transform;
                    owner.SetParent(other.gameObject.transform);
                }
            }
            else
            {
                //owner.transform.parent = other.gameObject.transform;
                owner.SetParent(other.gameObject.transform);
            }

            //Debug.Log("Stop");
            //Debug.Log("Detach");
            StopCoroutine("DetachTimer");
        }
        else if (other.CompareTag("Env_Props"))
        {
            owner.SetIsGround(true);
            owner.SetParent(null);
            StopCoroutine("DetachTimer");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enviroment") || other.CompareTag("Env_Props"))
        {
            owner.SetIsGround(false);

            time = 0.0f;
            StartCoroutine("DetachTimer");
        }
    }

    //public float GetCapsuleHeight() { return collider.height; }

    public void Lock()
    {
        //isLock = true;
        //StartCoroutine(LockTimer());
    }

    public void UnLock()
    {
        StopAllCoroutines();
        isLock = false;
    }

    IEnumerator LockTimer()
    {
        float time = 0.0f;
        while(time < lockTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        isLock = false;
    }

    IEnumerator DetachTimer()
    {
        time = 0.0f;

        while(time< detachTime)
        {
            time += Time.deltaTime;
            yield return null;
        }

        //Debug.Log("Detach");
        owner.SetParent(null);
        
    }

    public void RequestDetach()
    {
        StartCoroutine("DetachTimer");
    }
}
