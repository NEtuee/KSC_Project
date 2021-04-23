using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Hitable : Scanable
{
    public bool isOver = false;
    [SerializeField] protected float hp = 100f;
    public UnityEvent whenDestroy;
    public UnityEvent whenHit = new UnityEvent();
    public UnityEvent whenScanned = new UnityEvent();

    public abstract void Hit();

    public abstract void Hit(float damage);

    public abstract void Hit(float damage, out bool isDestroy);

    public abstract void Destroy();
}
