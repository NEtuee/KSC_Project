using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.InputSystem;

public class LevelLineUI : MonoBehaviour
{
    private Canvas _canvas;

    public enum State
    {
        Appear, Disappear, Done
    }

    [SerializeField] private State state = State.Done;
    [SerializeField] private FadeAppearImage backGround;
    [SerializeField] private FadeAppearImage alphabet;
    [SerializeField] private Sprite aLineSprite;
    [SerializeField] private Sprite bLineSprite;
    [SerializeField] private Sprite cLineSprite;
    [SerializeField] private EnumerateText levelLineText;
    [SerializeField] private string levelLineTargetText;
    [SerializeField] private EnumerateText bossNameText;
    [SerializeField] private string bossNameTargetText;
    [SerializeField] private float waitDisappearTime = 4f;

    private TimeCounterEx _timeCounter = new TimeCounterEx();

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvas.enabled = false;

        levelLineText.Init();
        bossNameText.Init();

        _timeCounter.CreateSequencer("Appear");
        _timeCounter.AddSequence("Appear", 0.0f, null, (value) =>
        {
            _canvas.enabled = true;
            backGround.Appear();
        });
        _timeCounter.AddSequence("Appear", backGround.AppearDuration, null, (value) =>
        {
            alphabet.Appear();

            levelLineText.SetTargetString(levelLineTargetText);
            bossNameText.SetTargetString(bossNameTargetText);
        });
        _timeCounter.AddSequence("Appear", alphabet.AppearDuration, null, (value) =>
        {
        });
        _timeCounter.AddSequence("Appear", waitDisappearTime, null, (value) =>
        {
            Dissapear();
        });

        _timeCounter.CreateSequencer("Dissapear");
        _timeCounter.AddSequence("Dissapear", 0.0f, null, (value) =>
        {
            levelLineText.TextFade(backGround.DisappearDuration);
            bossNameText.TextFade(backGround.DisappearDuration);
            backGround.Disappear();
            alphabet.Disappear();
        });

        _timeCounter.AddSequence("Dissapear", backGround.DisappearDuration, null, (value) =>
        {
            _canvas.enabled = false;
            state = State.Done;
        });

    }

    public void Appear()
    {
        state = State.Appear;
        levelLineText.Init();
        bossNameText.Init();
        _timeCounter.InitSequencer("Appear");
    }

    public void Dissapear()
    {
        state = State.Disappear;
        _timeCounter.InitSequencer("Dissapear");
    }

    public void Update()
    {
        if(state == State.Appear)
        {
            _timeCounter.ProcessSequencer("Appear", Time.deltaTime);
        }
        else if(state == State.Disappear)
        {
            _timeCounter.ProcessSequencer("Dissapear", Time.deltaTime);
        }

        //if (Keyboard.current.digit3Key.wasPressedThisFrame)
        //{
        //    Appear();
        //}

        //if(Keyboard.current.digit4Key.wasPressedThisFrame)
        //{
        //    Dissapear();
        //}
    }

    public void SetBossName(string name)
    {
        bossNameTargetText = name;
    }
}
