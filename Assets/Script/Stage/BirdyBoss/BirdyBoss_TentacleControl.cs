using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BirdyBoss_TentacleControl : MonoBehaviour
{
    [System.Serializable]
    public class TentacleClass
    {
        public List<MeshRenderer> graphics;
        public Material triggeredMaterial;
        public Material baseMaterial;
        public NewEmpShield shield;

        private int _current;

        public void Init()
        {
            _current = 0;
            foreach(var item in graphics)
            {
                item.material = baseMaterial;

            }
        }

        public bool IncreaseGague()
        {
            graphics[_current++].material = triggeredMaterial;
            if(_current >= graphics.Count)
            {
                shield.gameObject.SetActive(true);
                shield.VisibleVisual();

                return true;
            }

            return false;
        }

        public bool DecreaseGague()
        {
            graphics[--_current].material = baseMaterial;
            if(_current <= 0)
            {
                shield.Reactive();
                shield.gameObject.SetActive(false);
                return true;
            }

            return false;
        }
    }
    public BirdyBoss_HeadPattern head;
    public List<TentacleClass> tentacles;
    public float gagueIncreaseTerm = 1f;
    public float gagueDecreaseTerm = 0.3f;

    public UnityEngine.Events.UnityEvent whenDecrease;

    private TentacleClass _currentTentacle = null;
    private bool _increase = false;
    private bool _decrease = false;

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    public void Awake()
    {
        _timeCounter.CreateSequencer("Increase");
        _timeCounter.AddSequence("Increase", gagueIncreaseTerm, null, (x)=> {
            _increase = !_currentTentacle.IncreaseGague();
        });

        _timeCounter.CreateSequencer("Decrease");
        _timeCounter.AddSequence("Decrease", gagueDecreaseTerm, null, (x) => {
            _decrease = !_currentTentacle.DecreaseGague();
        });
    }

    public void Update()
    {
        if ((Keyboard.current.lKey.isPressed))
        {
            StartRandomTentacle();
        }

        if (_increase)
        {
            if(_timeCounter.ProcessSequencer("Increase", Time.deltaTime))
            {
                if(_increase)
                {
                    _timeCounter.InitSequencer("Increase");
                }
            }

        }
        
        if(_decrease)
        {
            if (_timeCounter.ProcessSequencer("Decrease", Time.deltaTime))
            {
                if (_decrease)
                {
                    _timeCounter.InitSequencer("Decrease");
                }
                else
                {
                    whenDecrease?.Invoke();
                    _currentTentacle = null;
                }
            }
        }
    }

    public bool IsTentacleActivate() { return _currentTentacle != null; }

    public void EndTentacle()
    {
        _increase = false;
        _decrease = true;

        _timeCounter.InitSequencer("Decrease");
    }

    public void StartRandomTentacle()
    {
        if (_currentTentacle != null || head.IsGroggy())
            return;

        _increase = true;
        _decrease = false;

        _currentTentacle = tentacles[Random.Range(0, tentacles.Count)];
        _currentTentacle.Init();

        _timeCounter.InitSequencer("Increase");
    }
    
    


}
