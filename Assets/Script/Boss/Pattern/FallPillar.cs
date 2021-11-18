using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class FallPillar : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Material _mat;
    [SerializeField] private float damage = 10.0f;
    [SerializeField] private float force = 300.0f;
    [SerializeField] private float fallPower = 1000.0f;
    [SerializeField] private float impactRadius = 3.0f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Transform impactPoint;
    private Cinemachine.CinemachineImpulseSource _impulseSource;
    private bool _falling = false;
    private bool _done = false;

    public bool Done => _done;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _mat = GetComponent<Renderer>().material;
        _mat.SetFloat("Dissvole", 1f);
        _impulseSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
        _rigidbody.isKinematic = true;
    }

    public void Fall()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.AddForce(Vector3.down * fallPower);
        _falling = true;
        _done = false;
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
        _falling = false;
        _done = false;
        StartCoroutine(Fade(time, 0.0f, null));
    }

    public void Disappear(float time)
    {
        StartCoroutine(Fade(time, 1.0f,()=> { _done = true; }));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_falling == true)
        {
            _rigidbody.isKinematic = true;
            _rigidbody.velocity = Vector3.zero;
            Disappear(4f);

            _impulseSource.GenerateImpulse();

            Collider[] playerColl = Physics.OverlapSphere(impactPoint.position, impactRadius, targetLayer);

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
