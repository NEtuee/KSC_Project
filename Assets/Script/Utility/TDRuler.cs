using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class TDRuler : MonoBehaviour
{
    public enum UnitType
    {
        Centimeter,
        Meter,
    };

    public UnitType unitType;
    public Vector3 size;
    public float graduationSize = 0.5f;
    public int labelDist = 10;

    #if UNITY_EDITOR
    void OnDrawGizmos() 
    {
        Handles.color = Color.green;
        Handles.DrawLine(transform.position, transform.position + transform.up * size.y);

        //int accur = (int)(size.y * (viewType == ViewType.Centimeter ? 10f : 1f));
        float accur = unitType == UnitType.Centimeter ? 10f : 1f;
        float dist = unitType == UnitType.Centimeter ? .1f : 1f;
        var tri = (transform.right + transform.forward).normalized;
        string unit = unitType == UnitType.Centimeter ? "cm" : "M";

        for(int i = 1; i <= (int)(size.y * accur); ++i)
        {
            var start = transform.position + transform.up * dist * (float)i;
            var end = start + -tri * graduationSize;

            if(i % 10 == 0)
            {
                end += -tri * graduationSize; 
            }

            if(i % labelDist == 0)
            {
                Handles.Label(start,"y_" + i + unit);
            }

            Handles.DrawLine(start, end);
        }

        Handles.color = Color.blue;
        Handles.DrawLine(transform.position, transform.position + transform.forward * size.z);

        for(int i = 1; i <= (int)(size.z * accur); ++i)
        {
            var start = transform.position + transform.forward * dist * (float)i;
            var end = start + -transform.right * graduationSize;

            if(i % 10 == 0)
            {
                end += -transform.right * graduationSize; 
            }

            if(i % labelDist == 0)
            {
                Handles.Label(start,"z_" + i + unit);
            }

            Handles.DrawLine(start, end);
        }

        Handles.color = Color.red;
        Handles.DrawLine(transform.position, transform.position + transform.right * size.x);

        for(int i = 1; i <= (int)(size.x * accur); ++i)
        {
            var start = transform.position + transform.right * dist * (float)i;
            var end = start + -transform.forward * graduationSize;

            if(i % 10 == 0)
            {
                end += -transform.forward * graduationSize; 
            }

            if(i % labelDist == 0)
            {
                Handles.Label(start,"x_" + i + unit);
            }

            Handles.DrawLine(start, end);
        }

    }
#endif
}
