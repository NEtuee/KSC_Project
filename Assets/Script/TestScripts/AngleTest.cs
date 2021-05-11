using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleTest : MonoBehaviour
{
    [SerializeField]private float result;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float climbingPlaneAngle = Vector3.Dot(Vector3.Cross(transform.up, Vector3.right), Vector3.forward);

        result = climbingPlaneAngle * Mathf.Rad2Deg;
        //Debug.Log(climbingPlaneAngle*Mathf.Rad2Deg);
    }
}
