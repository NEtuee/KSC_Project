using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : Hitable
{
    public GameObject destroyEffect;

    void Start()
    {
        base.Start();
    }

    void Update()
    {

    }

    public override void Destroy()
    {
        Destroy(Instantiate(destroyEffect, transform.position, transform.rotation), 3.5f);
        collider.enabled = false;
        renderer.enabled = false;
        isOver = true;

        whenDestroy.Invoke();
    }

    public override void Hit()
    {
    }

    public override void Hit(float damage)
    {
        hp -= damage;

        if (hp <= 0f)
        {
            Destroy();
        }
    }

    public override void Hit(float damage, out bool isDestroy)
    {
        hp -= damage;

        isDestroy = false;
        if(hp <= 0f)
        {
            isDestroy = true;
            Destroy();
        }
    }

    public override void Scanned()
    {
    }
}
