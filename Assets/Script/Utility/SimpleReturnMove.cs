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

    private void Start()
    {
        startPos = transform.position;
        endPos = transform.position + Vector3.right * dist;
        target = endPos;

        rigidbody = GetComponent<Rigidbody>();

        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while(true)
        {
            if(transform.position != target)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    
            }
            else
            {
                if(target == startPos)
                {
                    target = endPos;
                }
                else
                {
                    target = startPos;
                }
            }

            yield return null;
        }
    }
}
