using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODSoundManager : MonoBehaviour
{
    public SoundInfoItem infoItem;

    public bool createSoundMap = false;

    private Dictionary<int, SoundInfoItem.SoundInfo> _soundMap;
    private Dictionary<int, Queue<FMODUnity.StudioEventEmitter>> _cacheMap;

    private Dictionary<int, List<FMODUnity.StudioEventEmitter>> _activeMap;
    private Dictionary<int, FMOD.Studio.PARAMETER_DESCRIPTION> _globalCache;

    private void Start()
    {
        if(createSoundMap)
            CreateSoundMap();

        _cacheMap = new Dictionary<int, Queue<FMODUnity.StudioEventEmitter>>();
        _activeMap = new Dictionary<int, List<FMODUnity.StudioEventEmitter>>();
        _globalCache = new Dictionary<int, FMOD.Studio.PARAMETER_DESCRIPTION>();

        CreateCachedGlobalParams();
    }

    private void LateUpdate()
    {
        foreach(var pair in _activeMap)
        {
            var value = pair.Value;
            for(int i = 0; i < value.Count;)
            {
                if(!value[i].IsPlaying())
                {
                    _cacheMap[value[i].DataCode].Enqueue(value[i]);
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

    public FMODUnity.StudioEventEmitter Play(int id, Vector3 localPosition,Transform parent)
    {
        var emmiter = GetCache(id);

        emmiter.transform.SetParent(parent);
        emmiter.transform.localPosition = localPosition;
        emmiter.gameObject.SetActive(true);

        emmiter.Play();

        AddActiveMap(id,emmiter);
        
        return emmiter;
    }

    public FMODUnity.StudioEventEmitter Play(int id, Vector3 position)
    {
        var emmiter = GetCache(id);
        
        emmiter.transform.SetParent(null);
        emmiter.transform.SetPositionAndRotation(position,Quaternion.identity);
        emmiter.gameObject.SetActive(true);

        emmiter.Play();
        
        AddActiveMap(id,emmiter);

        return emmiter;
    }

    public void AddActiveMap(int id, FMODUnity.StudioEventEmitter emmiter)
    {
        if(_activeMap.ContainsKey(id))
        {
            _activeMap[id].Add(emmiter);
        }
        else
        {
            var list = new List<FMODUnity.StudioEventEmitter>();
            list.Add(emmiter);
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
        FMOD.RESULT result = FMODUnity.RuntimeManager.StudioSystem.setParameterByID(desc.id, value);
        if(result != FMOD.RESULT.OK)
        {
            Debug.Log("global parameter not found");
        }
    }

    public void ReturnCache(int id, FMODUnity.StudioEventEmitter emmiter)
    {
        emmiter.gameObject.SetActive(false);
        _cacheMap[id].Enqueue(emmiter);
    }

    public FMODUnity.StudioEventEmitter GetCache(int id)
    {
        if(!_cacheMap.ContainsKey(id) || _cacheMap[id].Count == 0)
        {
            CreateSoundCacheItem(id,1);
        }


        return _cacheMap[id].Dequeue();
    }

    public void CreateSoundCacheItem(int id,int count,bool active = false)
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

    public FMOD.Studio.PARAMETER_DESCRIPTION FindGlobalParamDesc(int id)
    {
        if(_globalCache.ContainsKey(id))
        {
            return _globalCache[id];
        }
        else
        {
            Debug.Log("global paramaeter does not exists");
            return default(FMOD.Studio.PARAMETER_DESCRIPTION);
        }
    }

    public SoundInfoItem.SoundInfo FindSoundInfo(int id)
    {
        if(createSoundMap)
        {
            if(_soundMap.ContainsKey(id))
                return _soundMap[id];
            else
            {
                Debug.LogError("Sound id not found");
                return null;
            }
        }
        else
        {
            return infoItem.FindSound(id);
        }
    }

    public void CreateCachedGlobalParams()
    {
        var global = infoItem.FindSound(0);
        
        foreach(var item in global.parameters)
        {
            FMOD.RESULT result = FMODUnity.RuntimeManager.StudioSystem.getParameterDescriptionByName(item.name, out var desc);

            if(result != FMOD.RESULT.OK)
            {
                Debug.Log("global Parameter does not exists : " + item.name);
                return;
            }

            _globalCache.Add(item.id,desc);
        }
    }

    public void CreateSoundMap()
    {
        if(_soundMap != null)
        {
            _soundMap.Clear();
        }

        _soundMap = new Dictionary<int, SoundInfoItem.SoundInfo>();

        var datas = infoItem.soundData;

        foreach(var data in datas)
        {
            _soundMap.Add(data.id,data);
        }
    }
}
