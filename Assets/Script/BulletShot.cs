using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShot : MonoBehaviour
{
    public GameObject bulletOrigin;

    public Transform shotPoint;

    public void Shot(Vector3 direction, float speed)
    {
        var bullet = Instantiate(bulletOrigin,shotPoint.position,Quaternion.identity).GetComponent<BulletBall>();
        bullet.direction = direction;
        bullet.speed = speed;
    }
}
