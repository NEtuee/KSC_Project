using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPGun : MonoBehaviour
{
    [SerializeField] private Animator gunAnim;
    [SerializeField] private Transform launchPos;
    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private ParticleSystem laserEffect;
    [SerializeField] private ParticlePool hitEffect;

    private Transform mainCamera;
    private RaycastHit hit;

    void Start()
    {
        mainCamera = Camera.main.transform;
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
        if(Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, 100.0f))
        {
            if (ReferenceEquals(laserEffect,null) == false)
                laserEffect.Play();

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
}
