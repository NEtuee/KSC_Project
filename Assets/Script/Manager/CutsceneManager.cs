using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CutsceneManager : ManagerBase
{
    public bool isPlaying = false;
    private LevelEdit_TimelinePlayer _currentTimeline;

    public override void Assign()
    {
        base.Assign();
        SaveMyNumber("CutsceneManager");

        AddAction(MessageTitles.cutscene_play, (x) =>
        {
            _currentTimeline = (LevelEdit_TimelinePlayer)x.data;
            isPlaying = true;
        });

        AddAction(MessageTitles.cutscene_stop, (x) =>
        {
            isPlaying = false;
        });

        AddAction(MessageTitles.cutscene_skip, (x) =>
        {
            SkipCutscene();
        });
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);
        if(isPlaying && Mouse.current.rightButton.wasPressedThisFrame && Keyboard.current.leftCtrlKey.isPressed)
        {
            if(_currentTimeline?.playableDirector.state == UnityEngine.Playables.PlayState.Playing)
            {
                SkipCutscene();
            }
        }
    }

    public void SkipCutscene()
    {
        _currentTimeline.playableDirector.time = _currentTimeline.endSignalTime;
        // _currentTimeline.playableDirector.Pause();
        // _currentTimeline?.End();
        
    }
}
