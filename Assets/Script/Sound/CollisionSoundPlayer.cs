using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSoundPlayer : MonoBehaviour
{
    public int code;
    public void OnCollisionEnter(Collision coll)
    {
        GameManager.Instance.soundManager.Play(code, Vector3.up, transform);
    }
}
