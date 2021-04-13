using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BooGieBottomCollision : Hitable
{
    public override void Hit()
    {
        whenHit.Invoke();
    }

    public override void Hit(float damage)
    {
        whenHit.Invoke();
    }

    public override void Hit(float damage, out bool isDestroy)
    {
        whenHit.Invoke();
        isDestroy = false;
    }

    public override void Destroy()
    {

    }

    public override void Scanned()
    {
        whenScanned.Invoke();
    }
}
