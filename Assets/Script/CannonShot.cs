using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShot : MonoBehaviour
{
    public delegate void Delegate();

    public Transform root;

    public float shotAngle = 180f;
    public float currentAngle = 0f;
    public Vector3 cannonRandomness = new Vector3(10f,0f,10f);
    public Transform target;
    public GameObject cannonBall;

    public Delegate whenDestroy = ()=>{};

    public bool _canShot = false;
    public bool canDestroy = false;

    private ExplosionTest _explosion;

    private void Start()
    {
        _explosion = GetComponent<ExplosionTest>();
        if(target == null)
        {
            target = GameManager.Instance.player.transform;
        }
    }

    public bool CanShot(Transform t)
    {
        target = t;

        var one = (target.position - transform.position).normalized;
        var two = transform.forward;

        one.y = 0f;
        two.y = 0f;

        currentAngle = Vector3.Angle(one,two);
        _canShot = currentAngle <= shotAngle;

        return _canShot;
    }

    public void DestroyCannon()
    {
        _canShot = false;
        root.gameObject.SetActive(false);
    }

    public void DestroyProgress()
    {
        _explosion.Exlposion(root.transform.position);
        whenDestroy();
        DestroyCannon();
    }

    public void Shot(GameObject explosion)
    {
        if(!_canShot)
            return;

        var obj = Instantiate(cannonBall,transform.position,Quaternion.identity).GetComponent<CannonBall>();
        obj.explosionParticle = explosion;
        obj.Shot(transform.position,target.position,cannonRandomness);

        //_explosion.Exlposion(root.transform.position);
    }

    public void Shot()
    {
        if (!_canShot)
            return;

 
        var obj = Instantiate(cannonBall, transform.position, Quaternion.identity).GetComponent<CannonBall>();
        obj.Shot(transform.position, target.position, cannonRandomness);

        //_explosion.Exlposion(root.transform.position);
    }

    private void OnTriggerStay(Collider coll)
    {
        if(coll.gameObject.tag == "Player" && canDestroy)
        {
            Debug.Log("CC");
            if(InputManager.Instance.GetAction(KeybindingActions.Interaction))
            //if(Input.GetKeyDown(KeyCode.F))
            {
                coll.transform.parent = null;

                DestroyProgress();
            }
        }
    }
}
