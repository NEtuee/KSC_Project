using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODSoundManager : MonoBehaviour
{
    public SoundInfoItem infoItem;

    public bool createSoundMap = false;

    private Dictionary<int, SoundInfoItem.SoundInfo> _soundMap;
    private Dictionary<int, Queue<FMODUnity.StudioEventEmitter>> _cacheMap;

    private Dictionary<int, List<FMODUnity.StudioEventEmitter>> _activeList;

    private void Start()
    {
        if(createSoundMap)
            CreateSoundMap();

        _cacheMap = new Dictionary<int, Queue<FMODUnity.StudioEventEmitter>>();
        _activeList = new Dictionary<int, List<FMODUnity.StudioEventEmitter>>();
        
    }

    public void Play(int id)
    {
        var emmiter = GetCache(id);

        emmiter.gameObject.SetActive(true);
        
    }

    public FMODUnity.StudioEventEmitter GetCache(int id)
    {
        if(!_cacheMap.ContainsKey(id))
        {
            CreateSoundCacheItem(id,1);
        }
        else if(_cacheMap[id].Count == 0)
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
            comp.gameObject.SetActive(active);

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
