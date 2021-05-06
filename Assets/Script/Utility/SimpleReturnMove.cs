using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleReturnMove : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 target;
    [SerializeField] private float dist = 20.0f;
    [SerializeField] private float speed = 5.0f;
    private Rigidbody rigidbody;

    [SerializeField] MoveType type;

    public enum MoveType
    {
        Horizontal,
        Vertical
    }

    [SerializeField]
    private void Start()
    {
        startPos = transform.position;

        switch (type)
        {
            case MoveType.Horizontal:
                 endPos = transform.position + Vector3.right * dist;
                break;
            case MoveType.Vertical:
                endPos = transform.position + Vector3.up * dist;
                break;
        }
        target = endPos;

        rigidbody = GetComponent<Rigidbody>();

        //StartCoroutine(Move());
    }

    //private void Update()
    //{
    //    if(transform.position != target)
    //        {
    //            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    
    //        }
    //        else
    //        {
    //            if(target == startPos)
    //            {
    //                target = endPos;
    //            }
    //            else
    //            {
    //                target = startPos;
    //            }
    //        }
    //}

    private void FixedUpdate()
    {
        if (transform.position != target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);

        }
        else
        {
            if (target == startPos)
            {
                target = endPos;
            }
            else
            {
                target = startPos;
            }
        }
    }

    // IEnumerator Move()
    // {
    //     while(true)
    //     {
    //         if(transform.position != target)
    //         {
    //             transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

    //         }
    //         else
    //         {
    //             if(target == startPos)
    //             {
    //                 target = endPos;
    //             }
    //             else
    //             {
    //                 target = startPos;
    //             }
    //         }

    //         yield return null;
    //     }
    // }
}
