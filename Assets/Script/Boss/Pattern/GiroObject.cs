using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GiroObject : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float damage = 10.0f;
    [SerializeField] private float force = 300.0f;
    private Rigidbody _rigidbody;
    private Material _mat;
    private Cinemachine.CinemachineImpulseSource _impulseSource;
    private bool _stop = false;

    public bool IsStop => _stop;
    

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _mat = GetComponent<Renderer>().material;
        _mat.SetFloat("Dissvole", 1f);
        _impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
        _rigidbody.isKinematic = true;
    }

    public void LaunchObject(Vector3 targetPosition, float power)
    {
        transform.SetParent(null);
        Vector3 dir = (targetPosition - transform.position).normalized;
        transform.LookAt(targetPosition);
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(dir * power);
    }

    public void Appear(float time)
    {
        _stop = false;
        StartCoroutine(Fade(time, 0.0f,null));
    }

    public void Disappear(float time)
    {
        StartCoroutine(Fade(time, 1.0f,()=>
        {
            _stop = true;
        }));
    }
    
    private IEnumerator Fade(float time,float target ,Action whenEnd)
    {
        float curTime = 0f;
        float initValue = _mat.GetFloat("Dissvole");

        while(curTime <= time)
        {
            _mat.SetFloat("Dissvole",Mathf.Lerp(initValue,target,curTime / time));
            curTime += Time.deltaTime;
            yield return null;
        }

        whenEnd?.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(_stop == false)
        {
            _rigidbody.isKinematic = true;
            _rigidbody.velocity = Vector3.zero;
            Disappear(2f);

            _impulseSource.GenerateImpulse();

            Collider[] playerColl = Physics.OverlapSphere(transform.position, 3f, targetLayer);

            if (playerColl.Length != 0)
            {
                foreach (Collider curr in playerColl)
                {

                    PlayerUnit player = curr.GetComponent<PlayerUnit>();
                    if (player != null)
                    {
                        if (player.GetState == PlayerUnit.ragdollState || player.GetState == PlayerUnit.respawnState)
                            continue;

                        player.TakeDamage(damage);
                        player.Ragdoll.ExplosionRagdoll(force, (player.Transform.position - transform.position).normalized);

                        break;
                    }
                }
            }
        }
    }
}
