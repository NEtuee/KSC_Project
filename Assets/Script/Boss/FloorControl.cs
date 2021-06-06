using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorControl : MonoBehaviour
{
    public List<Rotator> floors = new List<Rotator>();
    public Vector3 firstPosition;

    public bool positionning = false;
    private List<Vector3> origins = new List<Vector3>();

    public bool _launch = false;

    public Transform rimSoundPosition;

    public void Start()
    {
        if(positionning)
        {
            for(int i = 0; i < floors.Count; ++i)
            {
                origins.Add(floors[i].transform.position);
                floors[i].transform.position = firstPosition;
            }
        }
        
    }

    public void Update()
    {
        if (GameManager.Instance.PAUSE == true)
            return;

        if (_launch)
        {
            if(positionning)
            {
                for(int i = 0; i < floors.Count; ++i)
                {
                    floors[i].transform.position = Vector3.Slerp(floors[i].transform.position,origins[i],0.01f);
                }
            }
            
        }
    }

    public void Fall()
    {
        foreach(var floor in floors)
        {
            floor.Fall();
        }
    }

    public void StopMove()
    {
        foreach(var floor in floors)
        {
            floor.lerpSpeed = true;
            floor.SetStopMove(true);
        }
    }

    public void Launch()
    {
        if(_launch)
            return;

        _launch = true;
        foreach (var floor in floors)
        {
            floor.play = true;
        }
    }

    public void SpecialLaunch()
    {
        if(_launch)
            return;
        StartCoroutine(SoundLaunch());
    }

    IEnumerator SoundLaunch()
    {
        GameManager.Instance.soundManager.Play(2010, rimSoundPosition.position);
        yield return new WaitForSeconds(0.6f);

        _launch = true;
        foreach (var floor in floors)
        {
            floor.play = true;
        }

        yield return new WaitForSeconds(0.55f);
        GameManager.Instance.soundManager.Play(2011, rimSoundPosition.position);
        yield return new WaitForSeconds(0.65f);
        GameManager.Instance.soundManager.Play(2012, rimSoundPosition.position);
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.soundManager.Play(2013, rimSoundPosition.position);
    }
}
