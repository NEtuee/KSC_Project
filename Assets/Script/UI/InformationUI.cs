using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InformationUI : MonoBehaviour
{
    private Canvas _canvas;
    public enum State
    {
        Appear, Show, Disappear, Hide
    }

    [SerializeField] private State state = State.Hide;
    [SerializeField] private FadeAppearImage uiRoot;
    [SerializeField] private InformationScriptable descript;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private float showTime = 4f;

    public float ShowTime { get => showTime; set => showTime = value; }

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private Dictionary<string, InfomationText> informationDic = new Dictionary<string, InfomationText>();

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();

        for(int i = 0; i < descript.data.Count; i++)
        {
            informationDic.Add(descript.data[i].key, descript.data[i]);
        }

        _timeCounter.CreateSequencer("Appear");
        _timeCounter.AddSequence("Appear", 0.0f, null, (value) =>
        {
            state = State.Appear;
            uiRoot.Appear();
        });

        _timeCounter.AddSequence("Appear", uiRoot.AppearDuration, null, (value) =>
        {
            state = State.Show;
        });
        _timeCounter.AddSequence("Appear", showTime, null, (value) =>
        {
            state = State.Disappear;
            uiRoot.Disappear();
        });

        _timeCounter.AddSequence("Appear", uiRoot.DisappearDuration, null, (value) =>
        {
            state = State.Hide;
        });
    }

    public void Appear(string key)
    {
        state = State.Appear;
        _timeCounter.InitSequencer("Appear");

        if (informationDic.ContainsKey(key) == true)
        {
            if(PlayerUnit.GamepadMode == true)
            {
                text.text = informationDic[key].gamepad;
            }
            else
            {
                text.text = informationDic[key].keyboardMouse;
            }
        }
    }

    public void Update()
    {
        if (state == State.Hide)
            return;

        _timeCounter.ProcessSequencer("Appear", Time.deltaTime);
    }
}
