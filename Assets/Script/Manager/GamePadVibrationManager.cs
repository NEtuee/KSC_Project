using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MD;
using UnityEngine.InputSystem;

public class GamePadVibrationManager : ManagerBase
{
    [SerializeField] private GamepabVibrationSet gamepabVibrationSet;
    private TimeCounterEx _timeCounter;
    private bool _vibrating = false;
    private Dictionary<string, VibrationSet> _vibrationSetDic = new Dictionary<string, VibrationSet>();

    private PlayerUnit _playerUnit;
    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("GamepadVibrationManager");

        AddAction(MessageTitles.gamepadVibrationManager_vibration, (msg) =>
        {
            if (PlayerUnit.GamepadMode == false)
                return;

            _vibrating = true;
            var data = MessageDataPooling.CastData<VibrationData>(msg.data);
            _timeCounter.InitTimer("Vibration", 0f, data.time);
            Gamepad.current.SetMotorSpeeds(data.leftSpeed, data.rightSpeed);
        });

        AddAction(MessageTitles.gamepadVibrationManager_vibrationByKey, (msg) =>
        {
            if (PlayerUnit.GamepadMode == false)
                return;

            _vibrating = true;
            var key = (string)msg.data;
            if (_vibrationSetDic.ContainsKey(key) == false)
                return;

            var set = _vibrationSetDic[key];
            _timeCounter.InitTimer("Vibration", 0f, set.time);
            Gamepad.current.SetMotorSpeeds(set.leftSpeed, set.rightSpeed);
        });

        AddAction(MessageTitles.set_setplayer, (x) => {
            _playerUnit = (PlayerUnit)x.data;
        });

        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);
    }

    public override void Initialize()
    {
        base.Initialize();

        _timeCounter = new TimeCounterEx();
        _timeCounter.InitTimer("Vibration", 0f, 1f);

        if(gamepabVibrationSet != null)
        {
            for(int i = 0; i< gamepabVibrationSet.vibrationSets.Count; i++)
            {
                _vibrationSetDic[gamepabVibrationSet.vibrationSets[i].key] = gamepabVibrationSet.vibrationSets[i];
            }
        }
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        _timeCounter.IncreaseTimerSelf("Vibration", out bool limit, deltaTime);
        if(limit == true && _vibrating == true)
        {
            for(int i = 0; i< Gamepad.all.Count;i++)
            {
                Gamepad.all[i].SetMotorSpeeds(0.0f, 0.0f);
            }
            _vibrating = false;
        }
    }

    private void OnDisable()
    {
        if (PlayerUnit.GamepadMode == false)
            return;
        Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);
    }
}

namespace MD
{
    public class VibrationData : MessageData
    {
        public float leftSpeed;
        public float rightSpeed;
        public float time;
    }
}
