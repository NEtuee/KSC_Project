using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 speed;
    public bool play = true;
    public bool fall = false;
    private List<SubRotator> _subRotators;
    private List<Rigidbody> _bodys = new List<Rigidbody>();
    public void Start()
    {
        _subRotators = new List<SubRotator>(transform.GetComponentsInChildren<SubRotator>(false));
        _bodys = new List<Rigidbody>(transform.GetComponentsInChildren<Rigidbody>(false));
    }

    public void Update()
    {
        if (GameManager.Instance.GAMEUPDATE == GameManager.GameUpdate.Fixed)
            return;

        if (GameManager.Instance.PAUSE == true)
            return;

        if (play)
        {
            transform.rotation *= Quaternion.Euler(speed * Time.deltaTime);
            RotateSubRotators(Time.deltaTime);
        }

        if(fall)
        {
            AddGravity(Time.fixedDeltaTime);
        }
    }

    public void FixedUpdate()
    {
        if (GameManager.Instance.GAMEUPDATE == GameManager.GameUpdate.Update)
            return;

        if (GameManager.Instance.PAUSE == true)
            return;

        if (play)
        {
            transform.rotation *= Quaternion.Euler(speed * Time.fixedDeltaTime);
            RotateSubRotators(Time.fixedDeltaTime);
        }

        if(fall)
        {
            AddGravity(Time.fixedDeltaTime);
        }
    }

    public void Fall()
    {
        fall = true;
        foreach(var rig in _bodys)
        {
            rig.gameObject.GetComponent<MeshCollider>().enabled = false;
            rig.isKinematic = false;
            rig.useGravity = true;
            rig.transform.parent = null;
            rig.constraints = RigidbodyConstraints.None;

            rig.AddTorque(MathEx.RandomVector3(-50f,50f,-50f,50f,-50f,50f));
            //rig.AddForce(MathEx.RandomVector3(0f,10f,0f,10f,0f,10f));
        }
    }

    public void AddGravity(float deltaTime)
    {
        foreach(var rig in _bodys)
        {
            rig.AddForce(Vector3.down * 3f * deltaTime);
        }
    }

    public void RotateSubRotators(float deltaTime)
    {
        foreach(var rot in _subRotators)
        {
            rot.Spin(deltaTime);
        }
    }
}
