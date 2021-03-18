using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
public class RigCtrl : MonoBehaviour
{
    [SerializeField] private Rig aimingRig;
    [SerializeField] private List<Rig> rigs = new List<Rig>();
    [SerializeField] private float blendingSpeed = 3f;
    [SerializeField] private bool isBlending = false;

    public void Active()
    {
        if (isBlending == true)
            StopAllCoroutines();
        if(rigs.Count != 0)
            StartCoroutine(UpWeight());
    }

    public void Disable()
    {
        if (isBlending == true)
            StopAllCoroutines();
        if (rigs.Count != 0)
            StartCoroutine(DownWeight());
    }

    public void SetAimingWeight(float weight)
    {
        aimingRig.weight = weight;
    }

    IEnumerator UpWeight()
    {
        float targetWeight = 1f;
        float currentWeight = rigs[0].weight;
        isBlending = true;

        while(currentWeight < targetWeight)
        {
            currentWeight += blendingSpeed * Time.deltaTime;
            if (currentWeight > targetWeight)
                currentWeight = targetWeight;

            foreach (var rig in rigs)
                rig.weight = currentWeight;

            yield return null;
        }

        isBlending = false;
    }

    IEnumerator DownWeight()
    {
        isBlending = true;

        float targetWeight = 0f;
        float currentWeight = rigs[0].weight;

        while(currentWeight > targetWeight)
        {
            currentWeight -= blendingSpeed * Time.deltaTime;
            if (currentWeight < targetWeight)
                currentWeight = targetWeight;

            foreach (var rig in rigs)
                rig.weight = currentWeight;

            yield return null;
        }

        isBlending = false;
    }
}
