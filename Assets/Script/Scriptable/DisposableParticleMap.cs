using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DisposableParticlePair
{
    public string key;
    public GameObject particle;
}
[CreateAssetMenu(fileName = "ParticleMap", menuName = "ParticleMap")]

public class DisposableParticleMap : ScriptableObject
{
    public DisposableParticlePair[] pairs;
}
