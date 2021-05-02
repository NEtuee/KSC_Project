using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubRotator : MonoBehaviour
{
    public Vector3 speed;

    public void Spin(float deltaTime)
    {
        if (GameManager.Instance.GAMEUPDATE == GameManager.GameUpdate.Fixed)
            return;

        if (GameManager.Instance.PAUSE == true)
            return;

        transform.rotation *= Quaternion.Euler(speed * deltaTime);
    }
}
