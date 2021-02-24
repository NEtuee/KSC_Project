using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UniRx;

public class PlayerLockSystem : MonoBehaviour
{
    [SerializeField]private bool lockOn = false;
    private LockOnTarget currentTarget;
    [SerializeField] private RectTransform targetCrossHair;
    [SerializeField] private Transform drone;
    [SerializeField] private GameObject electric;
    [SerializeField] private BezierLightning lightning;
    private Image crossHairImage;
    [SerializeField] private float lockRange = 8.0f;
    [SerializeField] private float chargeSpeed = 30f;
    [SerializeField] private GageBarUI chargeGage;

    // Start is called before the first frame update
    void Start()
    {
        crossHairImage = targetCrossHair.GetComponent<Image>();
        GameManager.Instance.player.charge.Subscribe
            (value => chargeGage.SetValue(value / 100f));
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.Instance.GetAction(KeybindingActions.Interaction))
        {
            LockOnTarget prevTarget = currentTarget;

            foreach(LockOnTarget target in GameManager.Instance.lockOnTargets)
            {
                if(Vector3.Distance(target.transform.position,transform.position) > lockRange)
                {
                    continue;
                }

                if(currentTarget == null)
                {
                    lockOn = true;
                    currentTarget = target;
                    continue;
                }

                if(Vector2.SqrMagnitude(currentTarget.GetScreenPosition()-GameManager.Instance.GetScreenCenter())
                    > Vector2.SqrMagnitude(target.GetScreenPosition() - GameManager.Instance.GetScreenCenter()))
                {
                    currentTarget = target;
                }
            }

            if(currentTarget == null)
            {
                targetCrossHair.position = GameManager.Instance.GetScreenCenter();
                crossHairImage.DOFade(0.2f, 0.1f);
                lockOn = false;
            }
            else
            {
                crossHairImage.DOFade(0.8f, 0.1f);
                if (prevTarget == currentTarget)
                {
                    lockOn = false;
                    currentTarget = null;
                }
            }
        }

        if(currentTarget != null)
        {
            Vector2 screenPos = currentTarget.GetScreenPosition();
            //if (screenPos.x <= 0 || screenPos.x >= Screen.width || screenPos.y <= 0 || screenPos.y >= Screen.height)
            //{
            //    screenPos.x = Mathf.Clamp(screenPos.x, 0 + 100, Screen.width - 100);
            //    screenPos.y = Mathf.Clamp(screenPos.y, 0 + 100, Screen.height - 100);
            //}
            targetCrossHair.position = screenPos;
        }
        else
        {
            targetCrossHair.position = GameManager.Instance.GetScreenCenter();
        }
    

        if (InputManager.Instance.GetAction(KeybindingActions.Aiming))
        {
            GameManager.Instance.player.charge.Value += chargeSpeed * Time.deltaTime;
            GameManager.Instance.player.charge.Value = Mathf.Clamp(GameManager.Instance.player.charge.Value, 0.0f, 100f);
        }

        if(InputManager.Instance.GetAction(KeybindingActions.Shot))
        {
            var value = GameManager.Instance.player.charge.Value;
            GameManager.Instance.player.charge.Value = 0.0f;

            if (value == 100.0f)
            {
                if(lockOn)
                {
                    var dist = Vector3.Distance(currentTarget.transform.position,transform.position);
                    if(dist <= lockRange)
                    {
                        currentTarget.whenTriggerOn();


                        if(lightning == null)
                            return;
                        Destroy(Instantiate(electric,currentTarget.transform.position,Quaternion.identity),3f);

                        for(int i = 0; i < 7; ++i)
                        {
                            dist = Vector3.Distance(currentTarget.transform.position,drone.position);
                            lightning.Active(currentTarget.transform.position,drone.position,8,0.1f,dist * .4f);
                        }
                    }
                    else
                    {
                        if(lightning == null)
                            return;
                            
                        for(int i = 0; i < 4; ++i)
                        {
                            var random = MathEx.RandomCircle(1.5f);
                            dist = Vector3.Distance(random,drone.position);
                            lightning.Active(drone.position + random,drone.position,8,0.1f,dist * 10f);
                        }

                        Destroy(Instantiate(electric,drone.position,Quaternion.identity),3f);
                    }

                }
                else
                {

                    if(lightning == null)
                            return;
                            
                    for(int i = 0; i < 4; ++i)
                    {
                        var random = MathEx.RandomCircle(1.5f);
                        var dist = Vector3.Distance(random,drone.position);
                        lightning.Active(drone.position + random,drone.position,8,0.05f,dist * 10f);
                    }

                    Destroy(Instantiate(electric,drone.position,Quaternion.identity),3f);
                }
            }

        }

       
        
    }


}
