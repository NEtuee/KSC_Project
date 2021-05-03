using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
        Init();
        
        GameManager.Instance.AsynSceneManager.RegisterBeforeLoadOnStart(ReturnAllCache);

        Play(4000, Vector3.zero);
        Play(4001, Vector3.zero);
        Play(4002, Vector3.zero);
        Play(4003, Vector3.zero);

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

    public FMODUnity.StudioEventEmitter Play(int id, Vector3 localPosition,Transform parent)
    {
        var emitter = GetCache(id);

        emitter.transform.SetParent(parent);
        emitter.transform.localPosition = localPosition;
        emitter.gameObject.SetActive(true);

        emitter.Play();

        AddActiveMap(id,emitter);
        
        return emitter;
    }

    public void ReturnAllCache()
    {
        foreach(var pair in _activeMap)
        {
            var value = pair.Value;
            for(int i = 0; i < value.Count; ++i)
            {
                value[i].Stop();
                //_cacheMap[value[i].DataCode].Enqueue(value[i]);
                value[i].transform.SetParent(gameObject.transform);
                ReturnCache(pair.Key,value[i]);

            }
            
            value.Clear();
        }
    }

    private void Init()
    {
        if(createSoundMap)
            CreateSoundMap();

        _cacheMap = new Dictionary<int, Queue<FMODUnity.StudioEventEmitter>>();
        _activeMap = new Dictionary<int, List<FMODUnity.StudioEventEmitter>>();
        _globalCache = new Dictionary<int, FMOD.Studio.PARAMETER_DESCRIPTION>();

        CreateCachedGlobalParams();
    }

    public FMODUnity.StudioEventEmitter Play(int id, Vector3 position)
    {
        var emitter = GetCache(id);
        
        emitter.transform.SetParent(null);
        emitter.transform.SetPositionAndRotation(position,Quaternion.identity);
        emitter.gameObject.SetActive(true);

        emitter.Play();
        
        AddActiveMap(id,emitter);

        return emitter;
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
            Init();
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
}
