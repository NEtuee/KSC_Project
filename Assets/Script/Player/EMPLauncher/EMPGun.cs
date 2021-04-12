using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;


public class EMPGun : MonoBehaviour
{
    [SerializeField] private Animator gunAnim;
    [SerializeField] private Transform launchPos;
    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private ParticleSystem laserEffect;
    [SerializeField] private ParticlePool hitEffect;
    [SerializeField] private Transform laserEffectPos;
    [SerializeField] private MultiAimConstraint _aimConstraint;
    [SerializeField] private Animator playerAnim;

    private float aimWeight;
    
    private Transform mainCamera;
    private RaycastHit hit;

    void Start()
    {
        mainCamera = Camera.main.transform;
        aimWeight = _aimConstraint.weight;
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, 150f))
        {
            launchPos.LookAt(hit.point);
        }
        else
        {
            launchPos.LookAt(mainCamera.position + mainCamera.forward * 150.0f);
        }
    }

    public void LaunchLaser(float damage)
    {
        if (playerAnim != null)
        {
            playerAnim.SetTrigger("Shot");
            DOTween.To(() => aimWeight, x => { aimWeight = x;
                _aimConstraint.weight = aimWeight;
            }, 0f, 0.1f);
        }

        Vector3 effectPosition = launchPos.position + launchPos.forward * 15.2f;
        //Quaternion effectRot = laun
        
        if(Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, 100.0f))
        {
            // if (ReferenceEquals(laserEffect,null) == false)
            //     laserEffect.Play();
            GameManager.Instance.effectManager.Active("Laser",laserEffectPos.position,laserEffectPos.rotation);
            //hitEffect.Active(hit.point, Quaternion.identity);
            GameManager.Instance.effectManager.Active("LaserHit",hit.point);

            Hitable hitable;
            if (hit.collider.TryGetComponent<Hitable>(out hitable))
            {
                hitable.Hit(damage);
            }
        }
        else
        {
            if (laserEffect != null)
                laserEffect.Play();

            GameManager.Instance.effectManager.Active("Laser", laserEffectPos.position, laserEffectPos.rotation);

        }
    }

    public void LaunchLaser(float damage, out bool destroyed)
    {
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, 100.0f))
        {
            if (laserEffect != null)
                laserEffect.Play();

            if (hitEffect != null)
                hitEffect.Active(hit.point, Quaternion.identity);

            Hitable hitable;
            if (hit.collider.TryGetComponent<Hitable>(out hitable))
            {
                hitable.Hit(damage,out destroyed);
            }
            else
            {
                destroyed = false;
            }
        }
        else
        {
            if (laserEffect != null)
                laserEffect.Play();
            destroyed = false;
        }
    }

    public void LaunchImpact()
    {
        //impactEffect.Play();
        
    }

    public void Active(bool active)
    {
        this.gameObject.SetActive(active);
    }

    public void GunLoad()
    {
        gunAnim.SetTrigger("Next");
    }

    public void GunOff()
    {
        gunAnim.SetTrigger("Off");
    }

    public void EndShot()
    {
        DOTween.To(() => aimWeight, x => { aimWeight = x;
            _aimConstraint.weight = aimWeight; 
        }, 1f, 0.25f);
    }
}
