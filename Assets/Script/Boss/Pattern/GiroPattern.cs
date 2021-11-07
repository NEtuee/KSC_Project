using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GiroPattern : ObjectBase
{
    [SerializeField] private List<GiroObject> giroObjects = new List<GiroObject>();
    private List<Vector3> _initPosition = new List<Vector3>();
    [SerializeField] private Transform pivot;
    [SerializeField] private float _rotationAccelerationSpeed = 0.1f;

    private PlayerUnit _player;
    private Transform _target;

    private float _rotationSpeed = 0.0f;
    private bool play = false;
    private bool _rotate = false;
    //[SerializeField]private bool _done = true;

    public int ObjectCount => giroObjects.Count;

    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.set_setplayer, (x) => {
            _player = (PlayerUnit)x.data;
            _target = _player.transform;
        });

        _initPosition.Capacity = giroObjects.Count;
        for(int i = 0; i < giroObjects.Count; i++)
        {
            _initPosition.Add(giroObjects[i].transform.position);
        }

        //_timeCounter.CreateSequencer("Launch");

        //_timeCounter.AddSequence("Launch", 0.0f, null, (value) =>
        //{
        //    for (int i = 0; i < giroObjects.Count; i++)
        //    {
        //        giroObjects[i].Appear(2f);
        //    }
        //    _rotate = true;
        //});

        //_timeCounter.AddSequence("Launch", 3.0f, null, null);

        //for (int i = 0; i < giroObjects.Count; i++)
        //{
        //    int count = i;
        //    _timeCounter.AddSequence("Launch", 1f, null, (value) =>
        //     {
        //         giroObjects[count].LaunchObject(_target.position, 5000f);
        //     });
        //}

        //_timeCounter.InitSequencer("Launch");
    }

    public override void Initialize()
    {
        base.Initialize();

        RegisterRequest(GetSavedNumber("StageManager"));
        SendMessageQuick(MessageTitles.playermanager_sendplayerctrl, GetSavedNumber("PlayerManager"), null);
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);

        //if(Keyboard.current.rKey.wasPressedThisFrame)
        //{
        //    play = true;
        //}

        if(_rotate)
        {
            pivot.Rotate(Vector3.up * _rotationSpeed * deltaTime);
            _rotationSpeed += _rotationAccelerationSpeed * deltaTime;
        }
    }

    public override void FixedProgress(float deltaTime)
    {
        base.FixedProgress(deltaTime);

        //if(_done == false)
        //{
        //    bool allStop = true;
        //    for(int i = 0; i< giroObjects.Count; i++)
        //    {
        //        if (giroObjects[i].IsStop == false)
        //        {
        //            allStop = false;
        //            break;
        //        }   
        //    }

        //    if (allStop)
        //    {
        //        Debug.Log("allstop");
        //        pivot.gameObject.SetActive(false);
        //        for (int i = 0; i < giroObjects.Count; i++)
        //        {
        //            giroObjects[i].transform.SetParent(pivot);
        //            giroObjects[i].transform.position = _initPosition[i];
        //        }
        //        _rotationSpeed = 0.0f;
        //        _rotate = false;
        //        _done = true;
        //    }
        //}
    }

    public void Appear()
    {
        for (int i = 0; i < giroObjects.Count; i++)
        {
            giroObjects[i].transform.SetParent(pivot);
            giroObjects[i].transform.position = _initPosition[i];
        }
        _rotationSpeed = 0.0f;
        _rotate = false;

        pivot.gameObject.SetActive(true);

        for (int i = 0; i < giroObjects.Count; i++)
        {
            giroObjects[i].Appear(2f);
        }
        _rotate = true;
        //_done = false;
    }

    public void Launch(int index, Vector3 targetPosition, float power)
    {
        if (index < 0 || index >= giroObjects.Count)
            return;

        giroObjects[index].LaunchObject(targetPosition, power);
    }
}
