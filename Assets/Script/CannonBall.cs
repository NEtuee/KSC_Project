using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public GameObject DamageObject;
    public GameObject explosionParticle;

    public float damage;
    public float speed;
    public float height;
    private float _timer = 0f;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;

    private GameObject child;

    public void Shot(Vector3 position, Vector3 target, Vector3 randomness)
    {
        _timer = 0f;
        _startPosition = position;
        _targetPosition = target + MathEx.RandomVector3(-randomness,randomness);

        child = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        _timer += speed * Time.deltaTime;
        if(_timer >= 1f)
        {
            child.transform.parent = null;
            child.transform.localScale = Vector3.one;

            Destroy(Instantiate(explosionParticle,transform.position,Quaternion.identity),3.5f);
            Destroy(child,1f);
            Destroy(this.gameObject);

            Instantiate(DamageObject,transform.position,Quaternion.identity).GetComponent<Damage>().factor = damage;
        }
    }

    private void FixedUpdate()
    {
        var pos = Vector3.Lerp(_startPosition,_targetPosition,_timer);
        pos.y += height * Mathf.Sin(_timer * Mathf.PI);
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider coll)
    {
        PortalProgress portal = null;
        if(coll.gameObject.TryGetComponent<PortalProgress>(out portal))
        {
            Debug.Log("deleted");

            portal.WhenHit();

            Instantiate(DamageObject,transform.position,Quaternion.identity).GetComponent<Damage>().factor = damage;

            Destroy(Instantiate(explosionParticle,transform.position,Quaternion.identity),3.5f);
            Destroy(child,1f);
            Destroy(this.gameObject);
        }
    }
}
