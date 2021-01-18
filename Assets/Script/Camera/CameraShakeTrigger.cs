using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeTrigger : MonoBehaviour
{
    [SerializeField]private CameraCollision cameraCollision;
    [SerializeField]private float shakeFactor = 0f;
    [SerializeField]private float time = 0f;

    public void OnShakeByFactor()
    {
        //cameraCollision.OnShake(shakeFactor,time);
        GameManager.Instance.RequstCameraShakeByFactor(shakeFactor, time);
    }

    public void OnShakeByPos()
    {
        //cameraCollision.OnShake(transform.position);
        GameManager.Instance.RequstCameraShakeByPosition(transform.position);
    }
}
