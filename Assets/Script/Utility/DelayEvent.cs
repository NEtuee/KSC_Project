using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayEvent : MonoBehaviour
{
    public UnityEvent targetEvent;
    public float delayTime = 0f;

    IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
        targetEvent.Invoke();
    }

    public void Call()
    {
        StartCoroutine(Delay(delayTime));
    }
}
