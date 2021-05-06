using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 speed;
    public bool play = true;

    private List<SubRotator> _subRotators;

    public void Start()
    {
        _subRotators = new List<SubRotator>(transform.GetComponentsInChildren<SubRotator>(false));
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
    }

    public void RotateSubRotators(float deltaTime)
    {
        foreach(var rot in _subRotators)
        {
            rot.Spin(deltaTime);
        }
    }
}
