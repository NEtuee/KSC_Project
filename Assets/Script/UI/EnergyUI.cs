using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class EnergyUI : FadeUI
{
    [SerializeField] private List<GameObject> energyIcon = new List<GameObject>();
    private int _curCount = 0;

    protected new void Start()
    {
        if (visible == false)
        {
            visible = true;
            _currentVisibleTime = remainingVisibleTime;
        }

        for (int i = 0; i < energyIcon.Count; i++)
        {
            if (_curCount > i)
            {
                energyIcon[i].SetActive(true);
            }
            else
            {
                energyIcon[i].SetActive(false);
            }
        }

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                //Debug.Log((int)(_updateValue / 0.25f));
                int count = (int)(_updateValue / 0.25f);

                if (_curCount != count)
                {
                    _curCount = count;
                    for(int i = 0; i < energyIcon.Count; i++)
                    {
                        if(_curCount > i)
                        {
                            energyIcon[i].SetActive(true);
                        }
                        else
                        {
                            energyIcon[i].SetActive(false);
                        }
                    }
                }

                if (visible == false)
                    return;
                _currentVisibleTime -= Time.deltaTime;
                if (_currentVisibleTime < 0.0f) _currentVisibleTime = 0.0f;

                if (_currentVisibleTime <= 0.0f)
                {
                    visible = false;
                    Fade(0.0f, 1.0f);
                }
            });
    }
}
