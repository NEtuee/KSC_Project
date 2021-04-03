using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 speed;
    public bool play = true;

    public void Update()
    {
        if (GameManager.Instance.PAUSE == true)
            return;

        if (play)
            transform.rotation = transform.rotation * Quaternion.Euler(speed * Time.deltaTime);
    }
}
