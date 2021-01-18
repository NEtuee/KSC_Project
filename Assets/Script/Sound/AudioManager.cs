using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : SingletonMono<AudioManager>
{
    public enum AudioType
    {
        SFX,
        BGM,
        UI,
    }

    [System.Serializable]
    public class AudioItem
    {
        public string name;
        public AudioType type;
        public AudioClip clip;
    }

    public class AudioCacheItem
    {
        public AudioSource audioObject;

        public void SetGroup(AudioMixerGroup group)
        {
            audioObject.outputAudioMixerGroup = group;
        }

        public void SetClip(AudioClip clip)
        {
            audioObject.clip = clip;
        }

        public void Play(Vector3 pos, bool loop = false)
        {
            audioObject.transform.position = pos;
            Play(loop);
        }

        public void Play(bool loop = false)
        {
            audioObject.Stop();
            audioObject.volume = 1f;
            audioObject.loop = loop;
            audioObject.Play();
        }

        public bool IsPlaying()
        {
            return audioObject.isPlaying;
        }
    }

    [SerializeField]private List<AudioItem> audioItems = new List<AudioItem>();
    [SerializeField]private AudioMixer audioMixer;

    [SerializeField]private AudioMixerGroup sfxGroup;
    [SerializeField]private AudioMixerGroup bgmGroup;
    [SerializeField]private AudioMixerGroup uiGroup;

    [SerializeField]private float maxDistance = 1000f;

    private Queue<AudioCacheItem> audioCache = new Queue<AudioCacheItem>();
    private List<AudioCacheItem> playingList = new List<AudioCacheItem>();

    private Dictionary<string, AudioMixerGroup> mixerGroup = new Dictionary<string, AudioMixerGroup>();

    private void Awake()
    {
        SetSingleton(this);

        for(int i = 0; i < 10; ++i)
        {
            audioCache.Enqueue(CreateAudioCacheItem());
        }
    }

    public void Start()
    {
        var objTp = Play("WindLoop",Vector3.zero,true).audioObject.transform;
        objTp.SetParent(GameManager.Instance.GetPlayerObject().transform);
        objTp.localPosition = Vector3.zero;
    }

    public void Update()
    {
        for(int i = 0; i < playingList.Count;)
        {
            if(!playingList[i].IsPlaying() && !playingList[i].audioObject.loop)
            {
                audioCache.Enqueue(playingList[i]);
                playingList.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }

    }

    public AudioItem FindAudioItem(string n)
    {
        var item = audioItems.Find(x=> x.name == n);

        return item;
    }

    // public void SetVolume(AudioType type, float value)
    // {
    //     audioMixer.
    // }

    // public AudioMixerGroup FindMixerGroup()
    // {
    //     if(mixerGroup.contains)
    // }

    public AudioCacheItem Play(string n, Vector3 pos, bool loop = false)
    {
        var item = FindAudioItem(n);
        if(item == null)
        {
            Debug.LogError("audio file does not exists");
        }

        var cache = GetCache();
        cache.SetGroup(GetAudioGroup(item.type));
        cache.SetClip(item.clip);
        cache.Play(pos,loop);

        playingList.Add(cache);

        return cache;
    }

    public AudioCacheItem Play(string n, bool loop = false)
    {
        var item = FindAudioItem(n);
        if(item == null)
        {
            Debug.LogError("audio file does not exists");
        }

        var cache = GetCache();
        cache.SetGroup(GetAudioGroup(item.type));
        cache.SetClip(item.clip);
        cache.Play(loop);

        playingList.Add(cache);

        return cache;
    }

    private AudioCacheItem CreateAudioCacheItem()
    {
        var obj = new GameObject("AudioItem");
        var comp = obj.AddComponent<AudioSource>();
        var item = new AudioCacheItem();
        item.audioObject = comp;

        comp.rolloffMode = AudioRolloffMode.Linear;
        comp.maxDistance = maxDistance;
        AnimationCurve ac = comp.GetCustomCurve(AudioSourceCurveType.SpatialBlend);
        Keyframe[] keys = new Keyframe[1];

        for(int i = 0; i < keys.Length; ++i)
        {
            keys[i].value = 1f;
        }

        ac.keys = keys;
        comp.SetCustomCurve(AudioSourceCurveType.SpatialBlend, ac);

        return item;
    }

    private AudioMixerGroup GetAudioGroup(AudioType type)
    {
        if(type == AudioType.BGM)
            return bgmGroup;
        else if(type == AudioType.SFX)
            return sfxGroup;
        else if(type == AudioType.UI)
            return uiGroup;

        return null;
    }

    private AudioCacheItem GetCache()
    {
        if(audioCache.Count == 0)
        {
            return CreateAudioCacheItem();
        }
        else
        {
            return audioCache.Dequeue();
        }
    }

    public AudioMixer GetMainMixer()
    {
        return audioMixer;
    }
}
