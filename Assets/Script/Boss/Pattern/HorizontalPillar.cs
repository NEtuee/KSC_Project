using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class HorizontalPillar : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Collider _collider;
    private Material _mat;
    [SerializeField] private float damage = 10.0f;
    [SerializeField] private float force = 300.0f;
    private bool _rush = false;
    private bool _visible = false;
    private bool _done = false;
    private Transform lookAtTarget;
    public bool Visible => _visible;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _mat = GetComponent<Renderer>().material;
        _mat.SetFloat("Dissvole", 0f);
        _rigidbody.isKinematic = true;
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
    }

    public void Launch(Vector3 target, float power)
    {
        _rigidbody.isKinematic = false;
        Vector3 dir = (target - transform.position);
        dir.y = 0.0f;
        transform.rotation = Quaternion.LookRotation(dir.normalized);
        _rigidbody.AddForce(dir.normalized * power);
        _rush = true;
        StartCoroutine(CheckTime(5.0f));
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

    public void FixedUpdate()
    {
        if (_visible == true && _rush == false && _done == false)
        {
            Vector3 dir = (lookAtTarget.position - transform.position);
            dir.y = 0.0f;
            transform.rotation = Quaternion.LookRotation(dir.normalized);
        }
    }

    public void Appear(float time)
    {
        _visible = true;
        _collider.isTrigger = false;
        _done = false;
        StartCoroutine(Fade(time, 1.0f, null));
    }

    public void Disappear(float time)
    {
        StartCoroutine(Fade(time, 0.0f, () => { _visible = false; }));
    }

    public void Disappear(float time, Action whenEnd)
    {
        StartCoroutine(Fade(time, 0.0f, whenEnd));
    }

    private IEnumerator CheckTime(float time)
    {
        yield return new WaitForSeconds(time);

        if(_rush == true)
        {
            Disappear(2f,()=>
            {
                _rush = false;
                _rigidbody.isKinematic = true;
                _rigidbody.velocity = Vector3.zero;
                _visible = false;
            });
        }
    }

    public void SetLookAtTarget(Transform target)
    {
        lookAtTarget = target;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_rush == true && collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log(collision.gameObject.name);

            _rush = false;
            _done = true;
            _rigidbody.isKinematic = true;
            _rigidbody.velocity = Vector3.zero;
            _collider.isTrigger = true;

            Disappear(2f);

            PlayerUnit player = collision.gameObject.GetComponent<PlayerUnit>();
            if (player != null)
            {
                if (player.GetState == PlayerUnit.ragdollState || player.GetState == PlayerUnit.respawnState)
                    return;

                player.TakeDamage(damage);
                player.Ragdoll.ExplosionRagdoll(force, transform.forward);
              
                var effectData = MessageDataPooling.GetMessageData<MD.EffectActiveData>();
                effectData.position = collision.GetContact(0).point; 
                effectData.rotation = Quaternion.identity;
                effectData.key = "MedusaHit";

                var msg = MessagePool.GetMessage();
                msg.Set(MessageTitles.effectmanager_activeeffect, UniqueNumberBase.GetSavedNumberStatic("EffectManager"), effectData, null);

                MasterManager.instance.HandleMessage(msg);


                MD.SoundPlayData soundData = MessageDataPooling.GetMessageData<MD.SoundPlayData>();
                soundData.id = 1537;
                soundData.position = transform.position;
                soundData.returnValue = false;
                soundData.dontStop = false;
                var soundmsg = MessagePool.GetMessage();
                soundmsg.Set(MessageTitles.fmod_play, UniqueNumberBase.GetSavedNumberStatic("FMODManager"), soundData, null);
                MasterManager.instance.HandleMessage(soundmsg);
            }
        }
        else if(_rush == true)
        {
            //Debug.Log(collision.gameObject.name);

            _rush = false;
            _done = true;

            _rigidbody.isKinematic = true;
            _rigidbody.velocity = Vector3.zero;
            _collider.isTrigger = true;

            Disappear(2f);
        }
    }
}
