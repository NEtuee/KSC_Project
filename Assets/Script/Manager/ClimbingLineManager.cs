using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingLineManager : ManagerBase
{
    public List<ClimbingLine> climbingLines = new List<ClimbingLine>();

    //public override void Assign()
    //{
    //    base.Assign();
    //    SaveMyNumber("ClimbingLineManager");

    //    AddAction(MessageTitles.climbingLineManager_getClimbingLineManager, (msg) =>
    //     {
    //         var target = (MessageReceiver)msg.sender;
    //         SendMessageQuick(target, MessageTitles.set_climbingLineManager,this);
    //     });
    //}

    protected override void Awake()
    {
        base.Awake();
        SaveMyNumber("ClimbingLineManager");
        RegisterRequest();

        AddAction(MessageTitles.climbingLineManager_getClimbingLineManager, (msg) =>
        {
            var target = (MessageReceiver)msg.sender;
            SendMessageQuick(target, MessageTitles.set_climbingLineManager, this);
        });

        SendMessageQuick(MessageTitles.set_climbingLineManager, GetSavedNumber("Player"), this);
    }

    public void AddClimbingLines(ClimbingLine climbingLine)
    {
        climbingLines.Add(climbingLine);
    }
}
