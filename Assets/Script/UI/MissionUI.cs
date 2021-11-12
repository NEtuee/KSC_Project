using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

public class MissionUI : MonoBehaviour
{
    private Canvas _canvas;
    public enum State
    {
        Appear, Disappear, Done
    }

    [SerializeField] private State state = State.Done;
    [SerializeField] private FadeAppearImage mark;
    [SerializeField] private FadeAppearImage exclamation;
    [SerializeField] private FadeAppearImage panel;
    [SerializeField] private EnumerateText titleText;
    [SerializeField] private EnumerateText descriptionText;

    [SerializeField] private float panelOriginHeight;
    [SerializeField] private float panelTargetHeight = 153f;
    [SerializeField] private float panelAppearDuration = 0.5f;
    private Vector2 panelOriginalSize;

    [SerializeField] private MissionTextScriptable missionTextScriptable;

    [Header("Text")]
    [TextArea] [SerializeField] private string titleTargetText;
    [TextArea] [SerializeField] private string descriptionTargetText;

    private Dictionary<string, MissionText> missionDescriptionDic = new Dictionary<string, MissionText>();
    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;

        for(int i = 0; i < missionTextScriptable.descriptions.Count; i++)
        {
            missionDescriptionDic.Add(missionTextScriptable.descriptions[i].key, missionTextScriptable.descriptions[i]);
        }

        titleText.Init();
        descriptionText.Init();

        panelOriginalSize = panel.RectTransform.sizeDelta;
        panel.RectTransform.sizeDelta = new Vector2(panelOriginalSize.x, 0.0f);

        _timeCounter.CreateSequencer("Appear");
        _timeCounter.AddSequence("Appear", 0.0f, null, (value) =>
        {
            _canvas.enabled = true;
            mark.Appear();
            exclamation.Appear();
            panel.Appear();
        });
        _timeCounter.AddSequence("Appear", mark.AppearDuration, null, (value) =>
        {
            panel.RectTransform.DOSizeDelta(new Vector2(panelOriginalSize.x, panelTargetHeight), panelAppearDuration).SetEase(Ease.OutBack);
        });
        _timeCounter.AddSequence("Appear", panelAppearDuration, null, (value) =>
        {
            titleText.SetTargetString(titleTargetText);
            descriptionText.SetTargetString(descriptionTargetText);
        });
        _timeCounter.AddSequence("Appear", 2f, null, (value) =>
        {
            state = State.Done;
        });

        _timeCounter.CreateSequencer("Dissapear");
        _timeCounter.AddSequence("Dissapear", 0.0f, null, (value) =>
        {
            titleText.TextFade(panelAppearDuration);
            descriptionText.TextFade(panelAppearDuration);
        });
        _timeCounter.AddSequence("Dissapear", panelAppearDuration, null, (value) =>
        {
            panel.RectTransform.DOSizeDelta(new Vector2(panelOriginalSize.x, panelOriginHeight), panelAppearDuration).SetEase(Ease.OutBack); ;
        });
        _timeCounter.AddSequence("Dissapear", panelAppearDuration, null, (value) =>
        {
            mark.Disappear();
            exclamation.Disappear();
            panel.Disappear();
        });
        _timeCounter.AddSequence("Dissapear", mark.DisappearDuration, null, (value) =>
        {
            _canvas.enabled = false;
        });
    }

    public void Appear()
    {
        state = State.Appear;
        titleText.Init();
        descriptionText.Init();
        _timeCounter.InitSequencer("Appear");
    }

    public void Dissapear()
    {
        state = State.Disappear;
        _timeCounter.InitSequencer("Dissapear");
    }

    public void Update()
    {
        if (state == State.Appear)
        {
            _timeCounter.ProcessSequencer("Appear", Time.deltaTime);
        }
        else if (state == State.Disappear)
        {
            _timeCounter.ProcessSequencer("Dissapear", Time.deltaTime);
        }

        //if (Keyboard.current.digit3Key.wasPressedThisFrame)
        //{
        //    Appear();
        //}

        //if (Keyboard.current.digit4Key.wasPressedThisFrame)
        //{
        //    Dissapear();
        //}
    }

    public void SetText(string title, string description)
    {
        titleTargetText = title;
        descriptionTargetText = description;
    }

    public void SetText(string key)
    {
        if (missionDescriptionDic.ContainsKey(key) == false)
            return;

        var description = missionDescriptionDic[key];
        titleTargetText = description.title;
        descriptionTargetText = description.description;
    }
}
