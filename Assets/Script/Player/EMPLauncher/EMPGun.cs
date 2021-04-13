using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;


public class EMPGun : MonoBehaviour
{
    [SerializeField] private GameObject _gunObject;
    [SerializeField] private Animator gunAnim;
    [SerializeField] private Animator playerAnim;
    [SerializeField] private Transform launchPos;
    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private ParticleSystem laserEffect;
    [SerializeField] private Transform laserEffectPos;
    [SerializeField] private Transform lookAim;

    private float aimWeight;
    
    private Transform mainCamera;
    private RaycastHit hit;

    void Start()
    {
        mainCamera = Camera.main.transform;
        _gunObject.SetActive(false);
    }

    void Update()
    {
        launchPos.LookAt(lookAim);

        if (Input.GetKeyDown(KeyCode.K))
        {
            GameManager.Instance.cameraManager.GenerateRecoilImpulse();
        }
    }

    public void LaunchLaser(float damage)
    {
        if (playerAnim != null)
        {
            playerAnim.SetTrigger("Shot");
        }
        
        GameManager.Instance.cameraManager.GenerateRecoilImpulse();
        GameManager.Instance.effectManager.Active("Laser", laserEffectPos.position, laserEffectPos.rotation);

        if(Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, 100.0f))
        {
            GameManager.Instance.effectManager.Active("LaserHit",hit.point);

            if (hit.collider.TryGetComponent<Hitable>(out Hitable hitable))
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
            GameManager.Instance.effectManager.Active("Laser",laserEffectPos.position,laserEffectPos.rotation);
            GameManager.Instance.effectManager.Active("LaserHit",hit.point);

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
        //this.gameObject.SetActive(active);
        if (_gunObject != null)
        {
            _gunObject.SetActive(active);
        }
    }

    public void GunLoad()
    {
        if (gunAnim != null)
        {
            gunAnim.SetTrigger("Next");
        }
    }

    public void GunOff()
    {
        if (gunAnim != null)
        {
            gunAnim.SetTrigger("Off");
        }
    }

    public void EndShot()
    {

    }
}
