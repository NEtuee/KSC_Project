using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : ManagerBase
{
    public DisposableParticleMap particlePairReference;

    private Dictionary<string, ParticlePool> _particleDic = new Dictionary<string, ParticlePool>();

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("EffectManager");

        AddAction(MessageTitles.effectmanager_activeeffect, (msg) =>
        {
            EffectActiveData data = (EffectActiveData)msg.data;
            Active(data.key, data.position);
        });

        AddAction(MessageTitles.effectmanager_activeeffectwithrotation, (msg) =>
        {
            EffectActiveData data = (EffectActiveData)msg.data;
            Active(data.key, data.position,data.rotation);
        });

        AddAction(MessageTitles.effectmanager_activeeffectsetparent, (msg) =>
        {
            EffectActiveData data = (EffectActiveData)msg.data;
            Active(data.key, data.position, data.rotation).transform.SetParent(data.parent);
        });
    }

    private void Start()
    {
        for (int i = 0; i < particlePairReference.pairs.Length; i++)
        {
            ParticlePool particlePool = gameObject.AddComponent<ParticlePool>();
            particlePool.baseObject = particlePairReference.pairs[i].particle;
            particlePool.Init();
            particlePool.Active(new Vector3(10000.0f,10000.0f,10000.0f), Quaternion.identity);
            _particleDic.Add(particlePairReference.pairs[i].key,particlePool);
        }
    }

    public ParticleSystem Active(string key,Vector3 position, Quaternion rotation)
    {
        //Debug.Log(key);
        if (_particleDic.ContainsKey(key) == false)
        {
            Debug.LogWarning("Not Exist EffectKey");
            return null;
        }

        return _particleDic[key].Active(position,rotation);
    }
    
    public ParticleSystem Active(string key,Vector3 position)
    {
        //Debug.Log(key);
        if (_particleDic.ContainsKey(key) == false)
            return null;
        
        return _particleDic[key].Active(position, Quaternion.identity);
    }
}

struct EffectActiveData
{
    public string key;
    public Vector3 position;
    public Quaternion rotation;
    public Transform parent;
}

