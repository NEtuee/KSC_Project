using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    [SerializeField]private List<string> playList = new List<string>();

    public void PlayAll()
    {
        for(int i = 0; i < playList.Count; ++i)
        {
            Play(i);
        }
    }

    public void Play(int i)
    {
        AudioManager.instance.Play(playList[i],transform.position);
    }
}
