using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : ObjectPoolBase<ParticleSystem>
{
    public void Awake()
    {
        _activeDelegate += (t,position,rotation) =>
        {
            t.transform.SetPositionAndRotation(position,rotation);
            t.gameObject.SetActive(true);
            t.Play(true);
        };
        
        _createDelegate += t =>
        {
            t.Stop();
        };
        
        _deleteProgressDelegate += t =>
        {
            t.gameObject.SetActive(false);
            t.transform.SetParent(this.transform);
            t.Stop();
        };
        
        _deleteCondition += t =>
        {
            return (!t.isEmitting && t.particleCount == 0);
        };
    }
}
