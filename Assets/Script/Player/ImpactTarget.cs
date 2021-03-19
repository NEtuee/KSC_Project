using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactTarget : MonoBehaviour
{
    public delegate void WhenTriggerOn();
    public bool oneshot = false;
    public WhenTriggerOn whenTriggerOn = () => { };

    private bool _triggered = false;

    public virtual void TriggerOn()
    {
        if (_triggered)
            return;

        if (oneshot)
        {
            _triggered = true;
        }

        whenTriggerOn();
    }
}
