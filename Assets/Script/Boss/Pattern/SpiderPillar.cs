using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpiderPillar : MonoBehaviour
{
    private Material _mat;

    private void Awake()
    {
        _mat = GetComponent<Renderer>().material;
        _mat.SetFloat("Dissvole", 0f);
    }

    private IEnumerator Fade(float time, float target, Action whenEnd)
    {
        float curTime = 0f;
        float initValue = _mat.GetFloat("Dissvole");

        while (curTime <= time)
        {
            _mat.SetFloat("Dissvole", Mathf.Lerp(initValue, target, curTime / time));
            curTime += Time.deltaTime;
            yield return null;
        }

        whenEnd?.Invoke();
    }

    public void Appear(float time)
    {
        StartCoroutine(Fade(time, 1.0f, null));
    }

    public void Disappear(float time)
    {
        StartCoroutine(Fade(time, 0.0f, null));
    }
}
