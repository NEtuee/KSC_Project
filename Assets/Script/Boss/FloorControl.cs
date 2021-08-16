using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorControl : UnTransfromObjectBase
{
    public List<Rotator> floors = new List<Rotator>();
    public Vector3 firstPosition;

    public bool positionning = false;
    private List<Vector3> origins = new List<Vector3>();

    public bool _launch = false;

    public Transform rimSoundPosition;

    public override void Initialize()
    {
        RegisterRequest(GetSavedNumber("StageManager"));
        if(positionning)
        {
            for(int i = 0; i < floors.Count; ++i)
            {
                origins.Add(floors[i].transform.position);
                floors[i].transform.position = firstPosition;
            }
        }
        
    }

    public override void Progress(float deltaTime)
    {
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
        SoundPlay(2010,null, rimSoundPosition.position);
        yield return new WaitForSeconds(0.6f);

        _launch = true;
        foreach (var floor in floors)
        {
            floor.play = true;
        }

        yield return new WaitForSeconds(0.55f);
        SoundPlay(2011,null, rimSoundPosition.position);
        yield return new WaitForSeconds(0.65f);
        SoundPlay(2012,null, rimSoundPosition.position);
        yield return new WaitForSeconds(1.0f);
        SoundPlay(2013,null, rimSoundPosition.position);
    }

    public void SoundPlay(int code, Transform parent, Vector3 position)
    {
        if(parent != null)
        {
            var data = MessageDataPooling.GetMessageData<MD.AttachSoundPlayData>();

            data.id = code;
            data.localPosition = (Vector3)position;
            data.parent = parent;
            data.returnValue = true;

            SendMessageEx(MessageTitles.fmod_attachPlay,UniqueNumberBase.GetSavedNumberStatic("FMODManager"),data);
        }
        else
        {
            var data = MessageDataPooling.GetMessageData<MD.SoundPlayData>();

            data.id = code;
            data.position = (Vector3)position;
            data.dontStop = false;
            data.returnValue = true;

            SendMessageEx(MessageTitles.fmod_play,UniqueNumberBase.GetSavedNumberStatic("FMODManager"),data);
        }
    }
}
