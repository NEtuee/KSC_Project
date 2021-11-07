using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalPillarPattern : ObjectBase
{
    [SerializeField] private float pillarForce = 5000.0f;
    private HorizontalObjectPool _pillarObjectPool;
    private TimeCounterEx _timeCounter = new TimeCounterEx();
    private List<Vector3> _pillarStartPoint = new List<Vector3>();
    private List<HorizontalPillar> _horizonPillars = new List<HorizontalPillar>();

    public const int HORIZONTAL_PILLAR_PATTERN_POINT_COUNT = 6;

    private PlayerUnit _player;
    private Transform _target;
    public override void Assign()
    {
        base.Assign();

        AddAction(MessageTitles.set_setplayer, (x) => {
            _player = (PlayerUnit)x.data;
            _target = _player.transform;
        });

        _pillarObjectPool = GetComponent<HorizontalObjectPool>();

        _pillarStartPoint.Capacity = 6;
        _horizonPillars.Capacity = 6;

        _timeCounter.CreateSequencer("Horizon");

        _timeCounter.AddSequence("Horizon",0.0f,null,(value)=>
        {
            for(int i = 0; i < HORIZONTAL_PILLAR_PATTERN_POINT_COUNT; i++)
            {
                _horizonPillars.Add(_pillarObjectPool.Active(_pillarStartPoint[i], Quaternion.identity));
            }
        });

        _timeCounter.AddSequence("Horizon", 5.0f, null, null);

        for (int i = 0; i < HORIZONTAL_PILLAR_PATTERN_POINT_COUNT; i++)
        {
            int count = i;
            _timeCounter.AddSequence("Horizon", 2.0f, null, (value) =>
            {
                _horizonPillars[count].Launch(_target.position, pillarForce);
            });
        }

        _timeCounter.AddSequence("Horizon", 10.0f, null, (value)=>
        {
            gameObject.SetActive(false);
        });

        _timeCounter.InitSequencer("Horizon");
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

        _timeCounter.ProcessSequencer("Horizon", deltaTime);
    }

    public void Respawn()
    {
        _timeCounter.InitSequencer("Horizon");
    }

    public void SetPoint(ref List<Transform> points)
    {
        _pillarStartPoint.Clear();
        for (int i = 0; i < points.Count; i++)
        {
            _pillarStartPoint.Add(points[i].position);
        }
    }
}
