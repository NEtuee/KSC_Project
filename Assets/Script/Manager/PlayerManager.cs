using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerManager : ManagerBase
{
    [SerializeField] private PlayerCtrl_Ver2 _player;
    [SerializeField] private EMPGun _emp;

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("PlayerManager");
    }

    public override void Initialize()
    {
        base.Initialize();

        _player.hp.Subscribe(value =>
        {
            StateBarSetValueType data;
            data.type = UIManager.StateBarType.HP;
            data.value = value;
            if (_player.GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
            {
                data.visible = false;
                SendMessageEx(MessageTitles.uimanager_setvaluestatebar, GetSavedNumber("UIManager"), data);
            }
            else
            {
                data.visible = true;
                SendMessageEx(MessageTitles.uimanager_setvaluestatebar, GetSavedNumber("UIManager"), data);
                SendMessageEx(MessageTitles.uimanager_setvisibleallstatebar, GetSavedNumber("UIManager"), true);
            }
        });

        _player.stamina.Subscribe(value =>
        {
            StateBarSetValueType data;
            data.type = UIManager.StateBarType.Stamina;
            data.value = value;
            if (_player.GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
            {
                data.visible = false;
                SendMessageEx(MessageTitles.uimanager_setvaluestatebar, GetSavedNumber("UIManager"), data);
            }
            else
            {
                data.visible = true;
                SendMessageEx(MessageTitles.uimanager_setvaluestatebar, GetSavedNumber("UIManager"), data);
                SendMessageEx(MessageTitles.uimanager_setvisibleallstatebar, GetSavedNumber("UIManager"), true);
            }
        });

        _player.energy.Subscribe(value =>
        {
            StateBarSetValueType data;
            data.type = UIManager.StateBarType.Energy;
            data.value = value;
            if (_player.GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
            {
                data.visible = false;
                SendMessageEx(MessageTitles.uimanager_setvaluestatebar, GetSavedNumber("UIManager"), data);
            }
            else
            {
                data.visible = true;
                SendMessageEx(MessageTitles.uimanager_setvaluestatebar, GetSavedNumber("UIManager"), data);
                SendMessageEx(MessageTitles.uimanager_setvisibleallstatebar, GetSavedNumber("UIManager"), true);
            }
        });

        _player.hpPackCount.Subscribe(value =>
        {
            HpPackValueType data;
            data.value = value;

            if (_player.GetState() == PlayerCtrl_Ver2.PlayerState.Aiming)
            {
                data.visible = false;
                SendMessageEx(MessageTitles.uimanager_setvaluehppackui, GetSavedNumber("UIManager"), data);
            }
            else
            {
                data.visible = true;
                SendMessageEx(MessageTitles.uimanager_setvaluehppackui, GetSavedNumber("UIManager"), data);
                SendMessageEx(MessageTitles.uimanager_setvisibleallstatebar, GetSavedNumber("UIManager"), true);
            }
        });

        _player.chargeTime.Subscribe(value => {
            if (value >= 3f)
            {
                //crossHair.Third();
                SendMessageEx(MessageTitles.uimanager_setcrosshairphase, GetSavedNumber("UIManager"), 3);
            }
            else if (value >= 2f)
            {
                //crossHair.Second();
                SendMessageEx(MessageTitles.uimanager_setcrosshairphase, GetSavedNumber("UIManager"), 2);
            }
            else if (value >= 1f)
            {
                //crossHair.First();
                SendMessageEx(MessageTitles.uimanager_setcrosshairphase, GetSavedNumber("UIManager"), 1);
            }
        });
    }
}
