using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public DisposableParticleMap particlePairReference;

    private Dictionary<string, ParticlePool> _particleDic = new Dictionary<string, ParticlePool>();
    
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
        if (_particleDic.ContainsKey(key) == false)
        {
            Debug.LogWarning("Not Exist EffectKey");
            return null;
        }

        return _particleDic[key].Active(position,rotation);
    }
    
    public ParticleSystem Active(string key,Vector3 position)
    {
        if (_particleDic.ContainsKey(key) == false)
            return null;
        
        return _particleDic[key].Active(position, Quaternion.identity);
    }
}

