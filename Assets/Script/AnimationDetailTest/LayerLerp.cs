using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerLerp : MonoBehaviour
{
    public bool run = false;
    public int layerIndex = 1;
    public float initWeight = 1f;
    public float targetWeight = 0f;
    public float lerpFactor = 0.1f;

    private Animator _animator;

    
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    
    void Update()
    {
        if(run)
        {
            _animator.SetLayerWeight(layerIndex,initWeight);
            run = false;
        }

        float weight = Mathf.Lerp(_animator.GetLayerWeight(layerIndex),targetWeight,lerpFactor);
        _animator.SetLayerWeight(layerIndex,weight);
    }
}
