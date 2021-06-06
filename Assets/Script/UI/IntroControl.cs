using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class IntroControl : MonoBehaviour
{
    public Image black;
    public Transform lightTransform;
    public Transform targetRotation;
    public Vector3 rot;

    private bool rotate = false;
    

    void Start()
    {
        black.DOFade(0.0f, 0.5f);
        StartCoroutine(Rotate());
    }

    void Update()
    {
        if(rotate == true)
        {
            lightTransform.Rotate(Vector3.up, 50f * Time.deltaTime, Space.World);
        }
    }

    IEnumerator Rotate()
    {
        float time = 0.0f;

        rotate = true;
        //while(time < 4f)
        //{
        //    //lightTransform.rotation = Quaternion.RotateTowards(lightTransform.rotation, targetRotation.rotation, 100f * Time.deltaTime);
        //    //lightTransform.Rotate(Vector3.up , 50f * Time.deltaTime,Space.World);
        //    time += Time.deltaTime;
        //    yield return null;
        //}
        yield return new WaitForSeconds(5.0f);
        rotate = false;

        black.DOFade(2.0f, 1.5f);
        yield return new WaitForSeconds(2.0f);

       
    }
}
