using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowAI : IKPathFollowBossBase
{
    public string path;
    public bool progress;
    public bool loop;

    public void Start()
    {
        GetPath(path);
        _pathLoop = loop;
    }

    void Update()
    {
        if (GameManager.Instance.GAMEUPDATE != GameManager.GameUpdate.Update)
            return;
        
        Progress(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GAMEUPDATE != GameManager.GameUpdate.Fixed)
            return;
        
        Progress(Time.fixedDeltaTime);
    }

    public void SetProgress(bool value)
    {
        progress = value;
    }

    public void Progress(float deltaTime)
    {
        if(!progress)
            return;
        
        FollowPath(deltaTime);
    }
}
