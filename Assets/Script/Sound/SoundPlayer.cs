using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public int code;

    public void Play()
    {
        GameManager.Instance.soundManager.Play(code,transform.position);
    }
}
