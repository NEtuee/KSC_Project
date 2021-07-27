using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using Debug = UnityEngine.Debug;

public struct SoundPlayData
{
    public int id;
    public Vector3 position;
    public bool returnValue;

    public SoundPlayData(int id, Vector3 position, bool returnValue)
    {
        this.id = id;
        this.position = position;
        this.returnValue = returnValue;
    }
}
public struct AttachSoundPlayData
{
    public int id;
    public Vector3 localPosition;
    public Transform parent;

    public bool returnValue;

    public AttachSoundPlayData(int id, Vector3 localPosition, Transform parent,bool returnValue)
    {
        this.id = id;
        this.localPosition = localPosition;
        this.parent = parent;
        this.returnValue = returnValue;
    }
}
public struct SetParameterData
{
    public int soundId;
    public int paramId;
    public float value;
}

[SerializeField]
public class FMODManager : ManagerBase
{
    public SoundInfoItem infoItem;

    public List<int> startPlayList = new List<int>();

    private Dictionary<int, SoundInfoItem.SoundInfo> _soundMap;
    private Dictionary<int, Queue<FMODUnity.StudioEventEmitter>> _cacheMap;

    private Dictionary<int, List<FMODUnity.StudioEventEmitter>> _activeMap;
    private Dictionary<int, FMOD.Studio.PARAMETER_DESCRIPTION> _globalCache;

    // private void Start()
    // {
        
    //     if(GameManager.Instance.asynSceneManager != null)
    //     GameManager.Instance.asynSceneManager.RegisterBeforeLoadOnStart(ReturnAllCache);

    //     Play(4000, Vector3.zero);
    //     Play(4001, Vector3.zero);
    //     Play(4002, Vector3.zero);
    //     Play(4003, Vector3.zero);
    // }

    public override void Assign()
    {
        base.Assign();

        SaveMyNumber("FMODManager");

        AddAction(MessageTitles.fmod_play,Play);
        AddAction(MessageTitles.fmod_attachPlay,AttachPlay);
        AddAction(MessageTitles.fmod_setParam,SetParam);
        AddAction(MessageTitles.fmod_setGlobalParam,SetGlobalParam);
        AddAction(MessageTitles.scene_beforeSceneChange,BeforeSceneLoad);
    }

    public override void Initialize()
    {
        CreateSoundMap();

        _cacheMap = new Dictionary<int, Queue<FMODUnity.StudioEventEmitter>>();
        _activeMap = new Dictionary<int, List<FMODUnity.StudioEventEmitter>>();
        _globalCache = new Dictionary<int, FMOD.Studio.PARAMETER_DESCRIPTION>();

        CreateCachedGlobalParams();

        foreach(var item in startPlayList)
        {
            Play(item,Vector3.zero);
        }

    }

    public override void AfterProgress(float deltaTime)
    {

        foreach(var pair in _activeMap)
        {
            var value = pair.Value;
            for(int i = 0; i < value.Count;)
            {
                if(value[i] == null)
                {
                    value.RemoveAt(i);
                }
                else if(!value[i].IsPlaying())
                {
                    //_cacheMap[value[i].DataCode].Enqueue(value[i]);
                    ReturnCache(pair.Key,value[i]);
                    value.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }
        }
    }

#region MessageCallback
    private void Play(Message msg)
    {
        var data = (SoundPlayData)msg.data;
        var emitter = Play(data.id,data.position);

        if(data.returnValue)
        {
            var send = MessagePack(MessageTitles.fmod_soundEmitter,((MessageReceiver)msg.sender).uniqueNumber,emitter);
            SendMessageQuick(send);
        }
    }

    private void AttachPlay(Message msg)
    {
        var data = (AttachSoundPlayData)msg.data;
        var emitter = Play(data.id,data.localPosition,data.parent);

        if (data.returnValue)
        {
            var send = MessagePack(MessageTitles.fmod_soundEmitter,((MessageReceiver)msg.sender).uniqueNumber,emitter);
            SendMessageQuick(send);
        }
    }

    private void SetParam(Message msg)
    {
        var data = (SetParameterData)msg.data;
        SetParam(data.soundId,data.paramId,data.value);
    }

    private void SetGlobalParam(Message msg)
    {
        var data = (SetParameterData)msg.data;
        SetGlobalParam(data.paramId,data.value);
    }

    private void BeforeSceneLoad(Message msg)
    {
        ReturnAllCache(false);
    }

#endregion

    public void ReturnAllCache(bool stop)
    {
        foreach(var pair in _activeMap)
        {
            var value = pair.Value;
            for(int i = 0; i < value.Count; ++i)
            {
                if(stop)
                    value[i].Stop();

                value[i].transform.SetParent(gameObject.transform);
                ReturnCache(pair.Key,value[i]);

            }
            
            value.Clear();
        }
    }

    public FMODUnity.StudioEventEmitter Play(int id, Vector3 position)
    {
        var emitter = GetCache(id);
        
        emitter.transform.SetParent(null);
        emitter.transform.SetPositionAndRotation(position,Quaternion.identity);
        emitter.gameObject.SetActive(true);

        emitter.Play();
        var info = FindSoundInfo(id);

        emitter.EventInstance.setVolume(info.defaultVolume);

        if(emitter.EventDescription.is3D(out var is3d) == FMOD.RESULT.OK)
        {
            emitter.OverrideAttenuation = info.overrideAttenuation && is3d;
            if(emitter.OverrideAttenuation)
            {
                emitter.OverrideMinDistance = info.overrideDistance.x;
                emitter.OverrideMaxDistance = info.overrideDistance.y;
            }
        }
        
        AddActiveMap(id,emitter);

        return emitter;
    }
    public FMODUnity.StudioEventEmitter Play(int id, Vector3 localPosition,Transform parent)
    {
        var emitter = GetCache(id);

        emitter.transform.SetParent(parent);
        emitter.transform.localPosition = localPosition;
        emitter.gameObject.SetActive(true);

        emitter.Play();

        var info = FindSoundInfo(id);

        emitter.EventInstance.setVolume(info.defaultVolume);

        if(emitter.EventDescription.is3D(out var is3d) == FMOD.RESULT.OK)
        {
            emitter.OverrideAttenuation = info.overrideAttenuation && is3d;
            if(emitter.OverrideAttenuation)
            {
                emitter.OverrideMinDistance = info.overrideDistance.x;
                emitter.OverrideMaxDistance = info.overrideDistance.y;
            }
        }
        

        AddActiveMap(id,emitter);
        
        return emitter;
    }

    public void Play(int id, Vector3 position, float deferredTime)
    {
        StartCoroutine(DeferredPlay(id, position, deferredTime));
    }

    IEnumerator DeferredPlay(int id, Vector3 position, float deferredTime)
    {
        yield return new WaitForSeconds(deferredTime);
        Play(id, position);
    }

    private void AddActiveMap(int id, FMODUnity.StudioEventEmitter emitter)
    {
        if(_activeMap.ContainsKey(id))
        {
            _activeMap[id].Add(emitter);
        }
        else
        {
            var list = new List<FMODUnity.StudioEventEmitter>
            {
                emitter
            };
            _activeMap.Add(id,list);
        }
    }

    public void SetParam(int soundID, int parameterID, float value)
    {
        var n = FindSoundInfo(soundID).FindParameter(parameterID);
        value = Mathf.Clamp(value,n.min,n.max);

        foreach(var list in _activeMap[soundID])
        {
            list.SetParameter(n.name,value);
        }

        foreach(var list in _cacheMap[soundID])
        {
            list.SetParameter(n.name,value);
        }
    }

    public void SetGlobalParam(int id, float value)
    {
        var desc = FindGlobalParamDesc(id);
        var result = FMODUnity.RuntimeManager.StudioSystem.setParameterByID(desc.id, value);
        if(result != FMOD.RESULT.OK)
        {
            Debug.Log("global parameter not found");
        }
        
        //Debug.Log("ID : "+id+" SetValue"+value);
    }
    
    public float GetGlobalParam(int id)
    {
        var desc = FindGlobalParamDesc(id);
        RESULT result = FMODUnity.RuntimeManager.StudioSystem.getParameterByID(desc.id, out var value);
        if(result != FMOD.RESULT.OK)
        {
            Debug.Log("global parameter not found");
        }
        return value;
    }

    public Dictionary<int, List<FMODUnity.StudioEventEmitter>> GetActiveMap()
    {
        return _activeMap;
    }

    private void ReturnCache(int id, FMODUnity.StudioEventEmitter emitter)
    {
        emitter.gameObject.SetActive(false);
        emitter.transform.SetParent(this.transform);
        _cacheMap[id].Enqueue(emitter);
    }

    private FMODUnity.StudioEventEmitter GetCache(int id)
    {
        if (_cacheMap == null)
        {
            Debug.Log("cachemap Missing");
        }
        
        if(!_cacheMap.ContainsKey(id) || _cacheMap[id].Count == 0)
        {
            CreateSoundCacheItem(id,1);
        }

        return _cacheMap[id].Dequeue();
    }

    private void CreateSoundCacheItem(int id,int count,bool active = false)
    {
        var sound = FindSoundInfo(id);
        if(sound == null)
            return;
        
        for(int i = 0; i < count; ++i)
        {
            var comp = new GameObject("Sound").AddComponent<FMODUnity.StudioEventEmitter>();
            comp.Event = sound.path;
            comp.Preload = true;
            comp.DataCode = id;
            comp.EventInstance.setVolume(sound.defaultVolume);
            comp.gameObject.SetActive(active);
            comp.transform.SetParent(this.transform);

            if(_cacheMap.ContainsKey(id))
            {
                _cacheMap[id].Enqueue(comp);
            }
            else
            {
                var queue = new Queue<FMODUnity.StudioEventEmitter>();
                queue.Enqueue(comp);
                _cacheMap.Add(id,queue);
            }
            
        }
    }

    private FMOD.Studio.PARAMETER_DESCRIPTION FindGlobalParamDesc(int id)
    {
        if(_globalCache.ContainsKey(id))
        {
            return _globalCache[id];
        }
        else
        {
            Debug.Log("global parameter does not exists");
            return default(FMOD.Studio.PARAMETER_DESCRIPTION);
        }
    }

    private SoundInfoItem.SoundInfo FindSoundInfo(int id)
    {
        if(_soundMap.ContainsKey(id))
            return _soundMap[id];
        else
        {
            Debug.LogError("Sound id not found");
            return null;
        }
    }

    public float GetParameterByName(string name)
    {
        var result = FMODUnity.RuntimeManager.StudioSystem.getParameterByName(name,out var value);
        if(result != FMOD.RESULT.OK)
        {
            Debug.Log("parameter does not exists : " + name);
            return -1f;
        }

        return value;
    }

    private void CreateCachedGlobalParams()
    {
        var global = infoItem.FindSound(0);
        
        foreach(var item in global.parameters)
        {
            var result = FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName(item.name, out var desc);

            if(result != FMOD.RESULT.OK)
            {
                Debug.Log("global Parameter does not exists : " + item.name);
                return;
            }

            _globalCache.Add(item.id,desc);
        }
    }

    private void CreateSoundMap()
    {
        if(_soundMap != null)
        {
            _soundMap.Clear();
        }

        _soundMap = new Dictionary<int, SoundInfoItem.SoundInfo>();

        var data = infoItem.soundData;

        foreach(var d in data)
        {
            _soundMap.Add(d.id,d);
        }
    }


#if UNITY_EDITOR

    public List<int> playerList = new List<int>();

#endif


}
