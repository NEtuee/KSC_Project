using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSoundPlayer : MonoBehaviour
{
    public int code;
    public int parameterCode;
    public float velocityFactor;
    public Rigidbody rig;


    public void OnCollisionEnter(Collision coll)
    {
        GameManager.Instance.soundManager.Play(code, Vector3.up, transform);
        GameManager.Instance.soundManager.SetParam(code,parameterCode,rig.velocity.magnitude * velocityFactor);
    }
}
