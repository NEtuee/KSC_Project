using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using UniRx;

public class EMPGun : MonoBehaviour
{
    [SerializeField] private GameObject _gunObject;
    [SerializeField] private Animator gunAnim;
    [SerializeField] private Animator playerAnim;
    [SerializeField] private Transform launchPos;
    [SerializeField] private Transform laserEffectPos;
    [SerializeField] private Transform lookAim;
    [SerializeField] private CrossHair crossHair;
    [SerializeField] private float layserRadius = 1.0f;
    [SerializeField] private LayerMask hitLayer;

    private float aimWeight;
    
    private Transform mainCamera;
    private RaycastHit hit;

    void Start()
    {
        mainCamera = Camera.main.transform;
        _gunObject.SetActive(false);
        playerAnim = GetComponent<Animator>();
        GetComponent<PlayerCtrl_Ver2>().chargeTime.Subscribe(value => { 
            if(value >= 3f)
            {
                crossHair.Third();
            }
            else if(value >= 2f)
            {
                crossHair.Second();
            }
            else if (value >= 1f)
            {
                crossHair.First();
            }
        });
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
        //GameManager.Instance.effectManager.Active("Laser02", laserEffectPos.position, laserEffectPos.rotation);
        //GameManager.Instance.effectManager.Active("Laser_Level2", laserEffectPos.position, laserEffectPos.rotation);

        if (damage <=40f)
        {
            GameManager.Instance.effectManager.Active("Laser02", laserEffectPos.position, laserEffectPos.rotation);
            InputManager.Instance.GamePadSetVibrate(0.2f,0.6f);
        }
        else if (damage <= 80f)
        {
            GameManager.Instance.effectManager.Active("Laser_Level2", laserEffectPos.position, laserEffectPos.rotation);
            InputManager.Instance.GamePadSetVibrate(0.3f,0.8f);
        }   
        else if (damage <= 120f)
        {
            GameManager.Instance.effectManager.Active("Laser_Level3", laserEffectPos.position, laserEffectPos.rotation);
            InputManager.Instance.GamePadSetVibrate(0.4f,1.0f);
        }
        
        
        if(Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, 100.0f))
        //if (Physics.SphereCast(mainCamera.position,layserRadius, mainCamera.forward, out hit, 1000.0f,hitLayer))
           {
            GameManager.Instance.effectManager.Active("LaserHit",hit.point);

            if (hit.collider.TryGetComponent<Hitable>(out Hitable hitable))
            {
                hitable.Hit(damage);
                crossHair.ActiveHitMark();
            }
           }

        crossHair.ActiveAnimation();

        if (gunAnim != null)
        {
            gunAnim.SetTrigger("ToZero");
        }
    }

    public void LaunchLaser(float damage, out bool destroyed)
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

            destroyed = true;
        }
        else
        {
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

        if(crossHair != null)
        {
            crossHair.SetActive(active);
        }

        if(gunAnim != null)
        {
            if(active == true)
                gunAnim.SetTrigger("Active");
            else
                gunAnim.SetTrigger("Disable");
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
