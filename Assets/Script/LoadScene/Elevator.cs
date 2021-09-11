using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : UnTransfromObjectBase
{
    public bool isExit = false;
    public string loadScene = "";
    private Collider _coll;

    private Animator _animator;

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));

        //_sceneManager = GameObject.FindObjectOfType<AsynSceneManager>();
        _animator = GetComponent<Animator>();
        _coll = GetComponent<Collider>();

        // if(!isExit)
        // {
        //     Open();
        // }
    }

    public void LoadSpecificScene()
    {
        StartCoroutine(LoadSpecificSceneCoroutine());
    }

    public void LoadNextScene()
    {
        StartCoroutine(LoadNextSceneCoroutine());
    }

    public void LoadOutScene()
    {
        StartCoroutine(LoadNextSceneNotAsynCoroutine());
    }

    public IEnumerator LoadSpecificSceneCoroutine()
    {
        WaitForSeconds se = new WaitForSeconds(2f);
        yield return se;

        MD.StringData data = MessageDataPooling.GetMessageData<MD.StringData>();
        data.value = loadScene;
        SendMessageEx(MessageTitles.scene_loadSpecificLevel, GetSavedNumber("SceneManager"), data);
        //_sceneManager.LoadNextlevel();
    }

    public IEnumerator LoadNextSceneCoroutine()
    {
        WaitForSeconds se = new WaitForSeconds(2f);
        yield return se;
        
        SendMessageEx(MessageTitles.scene_loadNextLevel,GetSavedNumber("SceneManager"),null);
        //_sceneManager.LoadNextlevel();
    }

    public IEnumerator LoadNextSceneNotAsynCoroutine()
    {
        WaitForSeconds se = new WaitForSeconds(2f);
        yield return se;

        MD.StringData data = MessageDataPooling.GetMessageData<MD.StringData>();
        data.value = "OutScene";
        SendMessageEx(MessageTitles.scene_loadSceneNotAsync, GetSavedNumber("SceneManager"), data);
        //_sceneManager.LoadNextlevel();
    }

    public void ToExit()
    {
        _coll.enabled = true;
        isExit = true;
    }

    public void Open()
    {
        _animator.SetTrigger("OpenTrigger");
        SoundPlay(2014,null, transform.position);
    }

    public void Close()
    {
        _animator.SetTrigger("CloseTrigger");
        SoundPlay(2015,null, transform.position);
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

    public void ObjectTeleport(Vector3 localPos, Quaternion localRot, Transform target, bool attatch = false)
    {
        target.SetParent(this.transform);
        target.localPosition = localPos;
        target.rotation = localRot;

        if(!attatch)
        {
            target.SetParent(null); 
        }
    }

    public void ObjectTeleport(Elevator inElevator, Transform target, bool attatch = false)
    {
        target.SetParent(inElevator.transform);
        
        var localPos = target.localPosition;
        var localRot = target.localRotation;

        ObjectTeleport(localPos,localRot,target,attatch);
    }
}
