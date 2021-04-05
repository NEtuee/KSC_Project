using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    public GameObject baseObject;

    private Queue<ParticleSystem> _cache = new Queue<ParticleSystem>();
    private List<ParticleSystem> _progressParticles = new List<ParticleSystem>();

    private void Update()
    {
        for(int i = 0; i < _progressParticles.Count;)
        {
            if(!_progressParticles[i].isEmitting && _progressParticles[i].particleCount == 0)
            {
                _progressParticles[i].gameObject.SetActive(false);
                _progressParticles[i].Stop();

                _cache.Enqueue(_progressParticles[i]);
                _progressParticles.RemoveAt(i);
            }
            else
            {
                ++i;
            }
        }
    }

    public void Active(Vector3 position, Quaternion rotation)
    {
        var particle = GetCachedItem();
        particle.transform.SetPositionAndRotation(position,rotation);
        particle.gameObject.SetActive(true);
        particle.Play(true);

        _progressParticles.Add(particle);
    }

    private ParticleSystem GetCachedItem()
    {
        if(_cache.Count == 0)
            CreateCacheItems(1);
        
        return _cache.Dequeue();
    }

    private void CreateCacheItems(int count)
    {
        for(int i = 0; i < count; ++i)
        {
            var obj = Instantiate(baseObject,Vector3.zero,Quaternion.identity);
            var particle = obj.GetComponent<ParticleSystem>();

            obj.SetActive(false);
            obj.transform.SetParent(this.transform);
            particle.Stop();

            _cache.Enqueue(particle);
        }
    }
}
