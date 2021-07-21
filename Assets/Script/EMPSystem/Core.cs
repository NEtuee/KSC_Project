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

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("ObjectManager"));
    }

    public override void Destroy()
    {
        //GameManager.Instance.effectManager.Active("CannonExplosion", transform.position);
        EffectActiveData data;
        data.key = "CannonExplosion";
        data.position = transform.position;
        data.rotation = Quaternion.identity;
        data.parent = null;
        SendMessageEx(MessageTitles.effectmanager_activeeffect, GetSavedNumber("EffectManager"),data);
        collider.enabled = false;
        renderer.enabled = false;
        isOver = true;

        whenDestroy.Invoke();
    }

    public void Reactive()
    {
        collider.enabled = true;
        renderer.enabled = true;
        isOver = false;
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
