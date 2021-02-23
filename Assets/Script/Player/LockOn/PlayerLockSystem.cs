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
    private Image crossHairImage;
    [SerializeField] private float lockRange = 8.0f;
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
            foreach(LockOnTarget target in GameManager.Instance.lockOnTargets)
            {
                currentTarget = null;
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

        if (lockOn == true)
        {
            if (InputManager.Instance.GetAction(KeybindingActions.Aiming))
            {
                GameManager.Instance.player.charge.Value += 30f * Time.deltaTime;
                GameManager.Instance.player.charge.Value = Mathf.Clamp(GameManager.Instance.player.charge.Value, 0.0f, 100f);
            }
            if(InputManager.Instance.GetAction(KeybindingActions.Shot))
            {
                if (GameManager.Instance.player.charge.Value == 100.0f)
                {
                    currentTarget.whenTriggerOn();
                    GameManager.Instance.player.charge.Value = 0.0f;
                }
                else
                {
                    GameManager.Instance.player.charge.Value = 0.0f;
                }
            }
        }

       
        
    }


}
