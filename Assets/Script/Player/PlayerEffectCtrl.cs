using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectCtrl : MonoBehaviour
{
    [Header("Setting Property")]
    [SerializeField] private ParticleSystem footStepEffect;

    [Header("Player Bone Setting")]
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;

    private List<ParticleSystem> footStepEffectList = new List<ParticleSystem>();
    private int currentCount = 0;
    private int poolingNum = 50;
    private Transform effectReposit;
    void Start()
    {
        if (GameObject.Find("EffectReposit") == null)
        {
            GameObject effectReposit = new GameObject("EffectReposit");
            effectReposit.transform.position = Vector3.zero;
            effectReposit.transform.rotation = Quaternion.identity;
            this.effectReposit = effectReposit.transform;
        }

        if (footStepEffect != null)
        {
            for(int i = 0; i<poolingNum;i++)
            {
                ParticleSystem cur = Instantiate(footStepEffect, Vector3.zero, Quaternion.identity);
                cur.transform.SetParent(effectReposit);
                cur.Stop();
                footStepEffectList.Add(cur);
            }
        }
        else
        {
            Debug.LogWarning("Not Set FootStepEffect Object!");
        }
    }

    public void PlayLeftFootStep()
    {
        if(footStepEffect != null && leftFoot != null)
        {
            PlayFootStepEffect(leftFoot);
        }
    }

    public void PlayRightFootStep()
    {
        if(footStepEffect != null && rightFoot != null)
        {
            PlayFootStepEffect(rightFoot);
        }
    }

    private void PlayFootStepEffect(Transform playTransform)
    {
        footStepEffectList[currentCount].transform.position = playTransform.position;
        footStepEffectList[currentCount].transform.rotation = playTransform.rotation;

        footStepEffectList[currentCount].Play();

        currentCount++;
        if(currentCount >= footStepEffectList.Count)
        {
            currentCount = 0;
        }
    }
}
